namespace M347_Data_Fetcher.Processes
{
    public class LocationValidator
    {
        //Default cityName
        public static string cityName = "Gais AR";

        public static string Validate()
        {
            var tcp = new TcpConnection();

            if (tcp.bytes != null)
            {
                return cityName = tcp.data;
            }
            else
            {
                return cityName = "Gais AR";
            }
        }
    }
}
