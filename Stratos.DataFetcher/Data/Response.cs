namespace Stratos.DataFetcher.Data
{
    public class Response
    {
        public int Id { get; set; }
        public string dateInserted { get; set; }

        required public Location Location { get; set; }

        required public Current Current { get; set; }
    }
}
