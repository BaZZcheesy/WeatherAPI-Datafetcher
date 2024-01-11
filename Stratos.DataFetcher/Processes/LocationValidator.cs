using Stratos.DataFetcher.Pages;
using System.Security.Cryptography.X509Certificates;

namespace Stratos.DataFetcher.Processes
{
    public class LocationValidator
    {
        //Default cityName
        public static string cityName { get; set; }

        public static string Validate()
        { 
            if (TcpConnection.data == "kill")
            {
                WeatherGetter.aTimer.Stop();
                WeatherFetcherStatus.isRunning = false;
                WeatherFetcherStatus.commandState = "kill";
                return cityName;
            }
            else if (cityName == "continue")
            {
                WeatherGetter.aTimer.Start();
                WeatherFetcherStatus.isRunning = false;
                WeatherFetcherStatus.commandState = "continue";
                return cityName;
            }
            else
            {
                // If the TcpListener got a location it returns the location, else it will return the current cityName
                if (TcpConnection.data != null)
                {
                    WeatherFetcherStatus.isRunning = true;
                    return cityName = TcpConnection.data;
                }
                else
                {
                    WeatherFetcherStatus.isRunning = true;
                    return cityName;
                }
            }
        }
    }
}
