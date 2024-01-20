using System.Timers;
using Marten;
using Newtonsoft.Json;
using Stratos.DataFetcher.Api;
using Stratos.DataFetcher.Data;
using Stratos.DataFetcher.Pages;

namespace Stratos.DataFetcher.Processes
{
    public class WeatherGetter
    {
        private string[] locations = {
            "Saint Gallen",
            "Zürichsee",
            "Bern",
            "Luzern",
            "Basel",
            "Genf",
            "Appenzell",
            "Gais",
            "Genf",
            "Fribourg",
            "Lugano",
            "New York",
            "Beijing",
            "Abuja"
        };

        private IWeatherApi _weatherApi;
        private IDocumentStore _store;
        public static System.Timers.Timer aTimer;
        public static bool isTimerRunning = false;

        // Constructor DI (dependency injection)
        public WeatherGetter(IWeatherApi weatherapi, IDocumentStore store)
        {
            _weatherApi = weatherapi;
            _store = store;
        }

        public void setTimers()
        {
            aTimer = new System.Timers.Timer(15000);
            aTimer.Elapsed += OnTimedEvent;
            isTimerRunning = true;
            aTimer.Start();
        }

        async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            await WeatherGet();
        }

        async Task WeatherGet()
        {
            Response? jsonConverted;

            string response = string.Empty;

            Console.WriteLine("Started pulling data");

            foreach (var location in locations)
            {
                try
                {
                    //Fetch data from WeatherAPI
                    response = await _weatherApi.GetWeather("b11d405f0b6f41f9abc101604232012", location);

                    WeatherFetcherStatus.isRunning = true;
                    jsonConverted = JsonConvert.DeserializeObject<Response>(response);

                    Console.WriteLine("Data pulled and converted");

                    // Open a session for querying, loading, and updating documents
                    await using var session = _store.LightweightSession();

                    Console.WriteLine("inserting into db");

                    var weather = new Response
                    {
                        dateInserted = DateTime.Now.ToString(),
                        Current = jsonConverted.Current,
                        Location = jsonConverted.Location
                    };

                    //Insert the instanciated weather object into the database
                    session.Store(weather);

                    await session.SaveChangesAsync();

                    Console.WriteLine("Insertion process complete");

                    //Changes variable that is needed to show the state of the service
                    WeatherFetcherStatus.isRunning = true;

                    //Console.WriteLine() for debugging and additional info
                    Console.WriteLine(DateTime.Now.ToString() + "DB Insert with location: " + weather.Location.Name + " successfull");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                    //Changes variable that is needed to show the state of the service
                    WeatherFetcherStatus.isRunning = false;
                }
            }
        }
    }
}
