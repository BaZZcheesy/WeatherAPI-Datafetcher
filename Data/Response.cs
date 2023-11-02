namespace M347_Data_Fetcher.Data
{
    public class Response
    {
        public int Id { get; set; }

        required public Location Location { get; set; }

        required public Current Current { get; set; }
    }
}
