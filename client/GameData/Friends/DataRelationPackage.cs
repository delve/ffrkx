using FFRKInspector.GameData.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKInspector.GameData.Friends
{
    class DataRelationPackage
    {
        [JsonProperty("target_profiles")]
        public DataTargetProfiles[] Profiles;

        [JsonProperty("user_relations")]
        public DataRelatedRW[] Users;
    }
}
