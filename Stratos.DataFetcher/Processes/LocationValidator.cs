namespace Stratos.DataFetcher.Processes
{
    public class LocationValidator
    {
        //Default cityName
        public static string cityName { get; set; }

        public static string Validate()
        {
            // If the TcpListener got a location it returns the location, else it will return the default cityName
            if (TcpConnection.data != null)
            {
                return cityName = TcpConnection.data;
            }
            else
            {
                return cityName = "Gais AR";
            }
        }
    }
}
