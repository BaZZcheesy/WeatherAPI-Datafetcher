using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace M347_Data_Fetcher.Data
{
    public class Condition
    {
        [JsonProperty("text")]
        public required string Text { get; set; }

        [JsonProperty("icon")]
        public required string Icon { get; set; }
    }
}
