using Newtonsoft.Json;

namespace M347_Data_Fetcher.Data
{
    public class Current
    {
        [JsonProperty("last_updated")]
        public required string LastUpdated { get; set; }

        [JsonProperty("temp_c")]
        public required decimal TempC { get; set; }

        [JsonProperty("condition")]
        public required Condition Condition { get; set; }

        [JsonProperty("wind_kph")]
        public required decimal WindKph { get; set; }

        [JsonProperty("humidity")]
        public required int Humidity { get; set; }

        [JsonProperty("feelslike_c")]
        public required decimal FeelsLikeC { get; set; }
    }
}
