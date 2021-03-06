﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FFRKInspector.GameData.Converters;

namespace FFRKInspector.GameData
{
    class DataWorld
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("series_id")]
        public uint SeriesId;

        [JsonProperty("Id")]
        public uint Id;

        [JsonProperty("type")]
        public ushort Type;

        [JsonProperty("kept_out_at")]
        [JsonConverter(typeof(EpochToDateTime))]
        public DateTime KeptOutAt;

        [JsonExtensionData]
        public Dictionary<string, JToken> UnknownValues;
    }
}
