using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Witai
{
    public class WitResponse
    {
        [JsonProperty("msg_id")]
        public string msg_id { get; set; }
        [JsonProperty("_text")]
        public string _text { get; set; }
        [JsonProperty("outcomes")]
        public List<WitOutcome> outcomes { get; set; }
        [JsonIgnore]
        public string error { get; set; }
    }

    public class WitOutcome
    {
        [JsonProperty("_text")]
        public string _text { get; set; }
        [JsonProperty("intent")]
        public string intent { get; set; }
        [JsonProperty("entities")]
        public Dictionary<string, JArray> entities { get; set; }
        [JsonProperty("confidence")]
        public double confidence { get; set; }
    }

    public class WitEntity
    {
        [JsonProperty("datetime")]
        public JArray datetime { get; set; }
    }
}
