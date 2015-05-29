﻿using FFRKInspector.GameData;
using FFRKInspector.GameData.Party;
using FFRKInspector.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKInspector.Analyzer
{
    class EquipmentAnalyzer
    {
        private static readonly int kDefaultN = 3;

        public class Result
        {
            public double Score;
            public bool IsValid;
        }

        private struct SynergyDefStatCombo
        {
            public RealmSynergy.Value Synergy;
            public AnalyzerSettings.DefensiveStat Stat;
        }

        private struct SynergyOffStatCombo
        {
            public RealmSynergy.Value Synergy;
            public AnalyzerSettings.OffensiveStat Stat;
        }

        private class AnalysisBuddy
        {
            public DataBuddyInformation Buddy;
            public List<AnalysisItem> UsableItems = new List<AnalysisItem>();
            public AnalyzerSettings.PartyMemberSettings Settings;

            public List<AnalysisItem>[,] TopNOffense;
            public List<AnalysisItem>[,] TopNDefense;
        }

        private class AnalysisItem
        {
            public DataEquipmentInformation Item;
            public List<AnalysisBuddy> WhoCanUse = new List<AnalysisBuddy>();
            public EquipStats SynergizedStats = new EquipStats();
            public EquipStats NonSynergizedStats = new EquipStats();
            public Result Result;
        }

        private AnalyzerSettings mSettings;
        private List<AnalysisItem> mItems;
        private List<AnalysisBuddy> mBuddies;
        private int mTopN = kDefaultN;

        public EquipmentAnalyzer(AnalyzerSettings Settings)
        {
            mSettings = Settings;
        }

        public EquipmentAnalyzer()
        {
            mSettings = AnalyzerSettings.DefaultSettings;
        }

        public int ItemRankThreshold
        {
            get { return mTopN; }
            set { mTopN = value; }
        }

        public DataEquipmentInformation[] Items
        {
            set
            {
                mItems.Clear();
                foreach (DataEquipmentInformation equip in value)
                {
                    AnalysisItem item = new AnalysisItem();
                    item.Item = equip;
                    mItems.Add(item);
                }
                UpdateWhoCanUse();
            }
        }

        public DataBuddyInformation[] Buddies
        {
            set
            {
                mBuddies.Clear();
                foreach (DataBuddyInformation buddy in value.Where(x => mSettings[x.BuddyId].Score))
                {
                    AnalysisBuddy instance = new AnalysisBuddy();
                    instance.Buddy = buddy;
                    instance.Settings = mSettings[buddy.BuddyId];
                    mBuddies.Add(instance);
                }
                UpdateWhoCanUse();
            }
        }

        private void UpdateWhoCanUse()
        {
            foreach (AnalysisBuddy buddy in mBuddies)
                buddy.UsableItems.Clear();
            foreach (AnalysisItem item in mItems)
                item.WhoCanUse.Clear();

            if (mItems.Count == 0 || mBuddies.Count == 0)
                return;

            // For each item, build the list of potential users and vice versa.
            foreach (AnalysisBuddy buddy in mBuddies)
            {
                DataCache.Characters.Key buddy_key = new DataCache.Characters.Key { Id = buddy.Buddy.BuddyId };
                DataCache.Characters.Data buddy_data = null;
                if (!FFRKProxy.Instance.Cache.Characters.TryGetValue(buddy_key, out buddy_data))
                    continue;
                foreach (AnalysisItem item in mItems)
                {
                    if (!buddy_data.UsableEquips.Contains(item.Item.Category))
                        continue;
                    item.WhoCanUse.Add(buddy);
                    buddy.UsableItems.Add(item);
                }
            }
        }

        public void Run()
        {
            // First run through and compute the effective stats of each item based
            // on the value of the item level consideration setting.
            foreach (AnalysisItem item in mItems)
            {
                // Lookup the item in the cache.  This gives us base stats and max stats, so we can
                // compute effective stats.
                DataCache.Items.Key item_key = new DataCache.Items.Key { ItemId = item.Item.EquipmentId };
                DataCache.Items.Data item_data = null;
                if (!FFRKProxy.Instance.Cache.Items.TryGetValue(item_key, out item_data))
                    continue;

                byte effective_level = item.Item.Level;
                switch (mSettings.LevelConsideration)
                {
                    case AnalyzerSettings.ItemLevelConsideration.Current:
                        effective_level = item.Item.Level;
                        break;
                    case AnalyzerSettings.ItemLevelConsideration.CurrentRankMaxLevel:
                        effective_level = item.Item.LevelMax;
                        break;
                    case AnalyzerSettings.ItemLevelConsideration.FullyMaxed:
                        effective_level = StatCalculator.MaxLevel(
                            StatCalculator.Evolve(item.Item.BaseRarity, SchemaConstants.EvolutionLevel.PlusPlus));
                        break;
                }

                item.NonSynergizedStats = StatCalculator.ComputeStatsForLevel(item.Item.BaseRarity, item_data.BaseStats, item_data.MaxStats, effective_level);
                effective_level = StatCalculator.EffectiveLevelWithSynergy(effective_level);
                item.SynergizedStats = StatCalculator.ComputeStatsForLevel(item.Item.BaseRarity, item_data.BaseStats, item_data.MaxStats, effective_level);
            }

            RealmSynergy.SynergyValue[] synergy_values = RealmSynergy.Values.ToArray();
            AnalyzerSettings.DefensiveStat[] defensive_stats = Enum.GetValues(typeof(AnalyzerSettings.DefensiveStat)).Cast<AnalyzerSettings.DefensiveStat>().ToArray();
            AnalyzerSettings.OffensiveStat[] offensive_stats = Enum.GetValues(typeof(AnalyzerSettings.OffensiveStat)).Cast<AnalyzerSettings.OffensiveStat>().ToArray();

            List<AnalysisItem>[,] best_defensive_items = new List<AnalysisItem>[synergy_values.Length,defensive_stats.Length];
            List<AnalysisItem>[,] best_offensive_items = new List<AnalysisItem>[synergy_values.Length,offensive_stats.Length];

            // Sort the item list MxN different ways, once for each combination of (realm,stat)
            Parallel.ForEach(CartesianProduct(synergy_values, defensive_stats),
                x =>
                {
                    RealmSynergy.SynergyValue synergy = x.Key;
                    AnalyzerSettings.DefensiveStat stat = x.Value;
                    List<AnalysisItem> sorted_items = new List<AnalysisItem>(mItems);
                    sorted_items.Sort((a,b) =>
                    {
                        EquipStats a_stats = (a.Item.SeriesId == synergy.GameSeries) ? a.SynergizedStats : a.NonSynergizedStats;
                        EquipStats b_stats = (b.Item.SeriesId == synergy.GameSeries) ? a.SynergizedStats : b.NonSynergizedStats;
                        short a_value = ChooseDefensiveStat(a_stats, stat);
                        short b_value = ChooseDefensiveStat(b_stats, stat);
                        return a_value.CompareTo(b_value);
                    });
                    best_defensive_items[(int)synergy.Realm + 1,(int)stat] = sorted_items;
                });

            Parallel.ForEach(CartesianProduct(synergy_values, offensive_stats),
                x =>
                {
                    RealmSynergy.SynergyValue synergy = x.Key;
                    AnalyzerSettings.OffensiveStat stat = x.Value;
                    List<AnalysisItem> sorted_items = new List<AnalysisItem>(mItems);
                    sorted_items.Sort((a, b) =>
                    {
                        EquipStats a_stats = (a.Item.SeriesId == synergy.GameSeries) ? a.SynergizedStats : a.NonSynergizedStats;
                        EquipStats b_stats = (b.Item.SeriesId == synergy.GameSeries) ? a.SynergizedStats : b.NonSynergizedStats;
                        short a_value = ChooseOffensiveStat(a_stats, stat);
                        short b_value = ChooseOffensiveStat(b_stats, stat);
                        return a_value.CompareTo(b_value);
                    });
                    best_offensive_items[(int)synergy.Realm + 1, (int)stat] = sorted_items;
                });

            // Compute the top N items for each character.
            Parallel.ForEach(mBuddies,
                buddy =>
                {
                    buddy.TopNDefense = new List<AnalysisItem>[synergy_values.Length, defensive_stats.Length];
                    buddy.TopNOffense = new List<AnalysisItem>[synergy_values.Length, offensive_stats.Length];
                    for (int x=0; x < best_defensive_items.GetLength(0); ++x)
                    {
                        for (int y=0; y < best_defensive_items.GetLength(1); ++y)
                        {
                            List<AnalysisItem> best_items = best_defensive_items[x, y];
                            buddy.TopNDefense[x,y] = best_items.TakeWhile(item => item.WhoCanUse.Contains(buddy)).Take(mTopN).ToList();
                        }
                    }
                    for (int x = 0; x < best_offensive_items.GetLength(0); ++x)
                    {
                        for (int y = 0; y < best_offensive_items.GetLength(1); ++y)
                        {
                            List<AnalysisItem> best_items = best_offensive_items[x, y];
                            buddy.TopNOffense[x, y] = best_items.TakeWhile(item => item.WhoCanUse.Contains(buddy)).Take(mTopN).ToList();
                        }
                    }
                });

            // Finally, go through each item and assign it a score based on how many times it appears in someone's Top N list.
            Parallel.ForEach(mItems,
                item =>
                {
                    bool is_weapon = false;
                    switch (item.Item.Type)
                    {
                        case SchemaConstants.ItemType.Weapon:
                            is_weapon = true;
                            break;
                        case SchemaConstants.ItemType.Armor:
                            is_weapon = false;
                            break;
                        default:
                            return;
                    }

                    // In the worst case the item appears 0 times in the top N for any character and any realm.  In the best
                    // case it appears somewhere in the top N for every character who can use it, and in every single realm.
                    // Furthermore, we weight appearances by their rank, so the max *score* is that value times the best
                    // possible rank.
                    int usable_by = item.WhoCanUse.Count;
                    int max_denormalized_score = mTopN * usable_by * synergy_values.Length;
                    int denormalized_score = 0;
                    foreach (AnalysisBuddy buddy in mBuddies)
                    {
                        AnalyzerSettings.PartyMemberSettings buddy_settings = mSettings[buddy.Buddy.BuddyId];
                        List<AnalysisItem>[,] rank_array = is_weapon ? buddy.TopNOffense : buddy.TopNDefense;
                        int stat_index = is_weapon ? (int)buddy_settings.OffensiveStat : (int)buddy_settings.DefensiveStat;

                        for (int realm = 0; realm < rank_array.GetLength(0); ++realm)
                        {
                            List<AnalysisItem> top_n_for_stat = rank_array[realm, stat_index];
                            int index = top_n_for_stat.IndexOf(item);
                            if (index == -1)
                                continue;
                            denormalized_score += top_n_for_stat.Count - index;
                        }
                    }
                    item.Result.IsValid = true;
                    item.Result.Score = (double)denormalized_score / (double)max_denormalized_score;
                });
        }

        private IEnumerable<KeyValuePair<T,U>> CartesianProduct<T,U>(IEnumerable<T> First, IEnumerable<U> Second)
        {
            foreach (T t in First)
            {
                foreach (U u in Second)
                    yield return new KeyValuePair<T, U>(t, u);
            }
        }

        private short ChooseOffensiveStat(EquipStats stats, AnalyzerSettings.OffensiveStat stat)
        {
            switch (stat)
            {
                case AnalyzerSettings.OffensiveStat.ATK:
                    return stats.Atk.GetValueOrDefault(0);
                case AnalyzerSettings.OffensiveStat.MAG:
                    return stats.Mag.GetValueOrDefault(0);
                case AnalyzerSettings.OffensiveStat.MND:
                    return stats.Mnd.GetValueOrDefault(0);
                default:
                    return stats.Atk.GetValueOrDefault(0);
            }
        }

        private short ChooseDefensiveStat(EquipStats stats, AnalyzerSettings.DefensiveStat stat)
        {
            switch (stat)
            {
                case AnalyzerSettings.DefensiveStat.DEF:
                    return stats.Def.GetValueOrDefault(0);
                case AnalyzerSettings.DefensiveStat.RES:
                    return stats.Res.GetValueOrDefault(0);
                default:
                    return stats.Def.GetValueOrDefault(0);
            }
        }
    }
}
