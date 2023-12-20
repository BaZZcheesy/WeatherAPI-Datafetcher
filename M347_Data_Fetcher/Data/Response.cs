using Newtonsoft.Json.Serialization;

namespace M347_Data_Fetcher.Data
{
    public class Response
    {
        public int Id { get; set; }
        public string dateInserted { get; set; }

        required public Location Location { get; set; }

        required public Current Current { get; set; }

        //JsonProperty("")
        //required public int ResponseCode { get; set; };
    }
}
