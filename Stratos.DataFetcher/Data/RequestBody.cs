using Newtonsoft.Json;

namespace Stratos.DataFetcher.Data
{
    public class RequestBody
    {
        [JsonProperty("state")]
        public required bool State { get; set; }
    }
}
