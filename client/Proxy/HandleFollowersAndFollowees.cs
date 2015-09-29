using FFRKInspector.Database;
using FFRKInspector.GameData;
using FFRKInspector.GameData.Friends;
using Fiddler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKInspector.Proxy
{
    /// <summary>
    /// When first opened the Friends list gets a /dff/relation/followee_and_follower_list response containing for 
    ///     followers and followees both:
    ///         the first 5 'target_profiles' (all the RW stats, gear, etc, & the player name, id, slogan, etc)
    ///         a list of all 'user_relations' (user_id mappings)
    /// As you scroll you get one response of /dff/relation/find_by_user_ids per page with 5 target_profiles and thier related user_relations records
    /// The find_by_user_ids responses do not indicate whether they correspond with followers or followees. That is apparently left up to the app
    /// 
    /// parsing:
    ///     build a DB of followees by userid, leave most of data blank until the user id comes up in a target_profile.
    ///     keep these data points from target_profile
    ///         nickname = player name (for identification in app list)
    ///         profile_message = slogan (for identification in app list)
    ///         user_id = matches to 'user_relations'
    ///         relation_status = 1 is follower, 2 is followee, 3 is mutual
    ///         supporter_buddy_soul_strike_name = name of the SB
    ///         stats:
    ///         supporter_buddy_acc = accuracy
    ///         supporter_buddy_atk = attack
    ///         supporter_buddy_matk = magic
    ///         supporter_buddy_mnd = mind
    ///         supporter_buddy_spd = speed
    /// </summary>
    class HandleFollowersAndFollowees : SimpleResponseHandler
    {
        public override bool CanHandle(Session Session)
        {
            string RequestPath = Session.oRequest.headers.RequestPath;
            return RequestPath.Equals("/dff/relation/followee_and_follower_list", StringComparison.CurrentCultureIgnoreCase)
                || RequestPath.Equals("/dff/relation/find_by_user_ids", StringComparison.CurrentCultureIgnoreCase);
        }

        public override void Handle(Session Session)
        {
            if (Session.oRequest.headers.RequestPath.Equals("/dff/relation/followee_and_follower_list", StringComparison.CurrentCultureIgnoreCase))
            {
                //We just opened the friends list. populate the user_relations list and add in the 5 target_profiles rcvd
                //parse json for follower and followee arrays, then parse those for target profiles and user relations, then merge into one array

                DataAllRelations followersFollowees = JsonConvert.DeserializeObject<DataAllRelations>(GetResponseBody(Session));
            } 
            else
            {
                //must be a /dff/relation/find_by_user_ids ; add the 5 new target_profiles
            }

            //DbOpInsertItems insert_request = new DbOpInsertItems();
            //foreach (DataEquipmentInformation equip in party.Equipments)
            //{
            //    DbOpInsertItems.ItemRecord record = new DbOpInsertItems.ItemRecord();
            //    record.EquipCategory = equip.Category;
            //    record.Id = equip.EquipmentId;
            //    record.Name = equip.Name.TrimEnd(' ', '+', '＋');
            //    record.Type = equip.Type;
            //    record.Rarity = equip.BaseRarity;
            //    record.Series = equip.SeriesId;
            //    insert_request.Items.Add(record);
            //}
            //FFRKProxy.Instance.Database.BeginExecuteRequest(insert_request);

            //FFRKProxy.Instance.GameState.PartyDetails = party;
            //FFRKProxy.Instance.RaisePartyList(party);
        }
    }
}
