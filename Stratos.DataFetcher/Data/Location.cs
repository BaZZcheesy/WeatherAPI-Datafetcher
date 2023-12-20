using Newtonsoft.Json;

namespace Stratos.DataFetcher.Data
{
    public class Location
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("region")]
        public required string Region { get; set; }

        [JsonProperty("country")]
        public required string Country { get; set; }

        [JsonProperty("lat")]
        public required decimal Lat { get; set; }

        [JsonProperty("lon")]
        public required decimal Lon { get; set; }

        [JsonProperty("localtime_epoch")]
        public required int LocaltimeEpoch { get; set; }

        [JsonProperty("localtime")]
        public required string Localtime { get; set; }
    }
}
