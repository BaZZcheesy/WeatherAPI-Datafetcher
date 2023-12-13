using M347_Data_Fetcher.Api;
using M347_Data_Fetcher.Data;
using M347_Data_Fetcher.Pages;
using Marten;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Timers;

namespace M347_Data_Fetcher.Processes
{
    public class WeatherGetter
    {
        private readonly IWeatherApi _weatherApi;
        private readonly IDocumentStore _store;
        private LocationValidator _locationValidator;

        void setTimers()
        {
            System.Timers.Timer aTimer;

            aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Start();
        }

        async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            await _locationValidator.Validate;
            WeatherGet(LocationValidator.cityName);
        }

        async Task WeatherGet(string cityName)
        {
            
            Response? jsonConverted;

            //TODO: create function to accept parameters from another GUI/User Interface
            string response = string.Empty;

            WeatherFetcherStatus.currentCity = cityName;

            try
            {
                //Fetch data from WeatherAPI
                response = await _weatherApi.GetWeather("1c39ca929b4b4d23a4364338231708", cityName);

                WeatherFetcherStatus.isRunning = true;
                jsonConverted = JsonConvert.DeserializeObject<Response>(response);

                // Open a session for querying, loading, and updating documents
                await using var session = _store.LightweightSession();

                var weather = new Response
                {
                    dateInserted = DateTime.Now.ToString(),
                    Current = jsonConverted.Current,
                    Location = jsonConverted.Location
                };

                //Insert the instanciated weather object into the database
                session.Store(weather);

                await session.SaveChangesAsync();

                //Changes variable that is needed to show the state of the service
                WeatherFetcherStatus.isRunning = true;

                //Console.WriteLine() for debugging and additional info
                Console.WriteLine(DateTime.Now.ToString() + "DB Insert with location: " + weather.Location.Name + " successfull");
            }
            catch (Exception e)
            {
                //Changes variable that is needed to show the state of the service
                WeatherFetcherStatus.isRunning = false;
            }
        }
    }
}
