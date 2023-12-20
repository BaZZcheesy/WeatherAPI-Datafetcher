using Newtonsoft.Json;

namespace Stratos.DataFetcher.Data
{
    public class Condition
    {
        [JsonProperty("text")]
        public required string Text { get; set; }

        [JsonProperty("icon")]
        public required string Icon { get; set; }
    }
}
