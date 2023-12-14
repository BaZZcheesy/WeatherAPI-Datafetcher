using M347_Data_Fetcher.Api;
using M347_Data_Fetcher.Processes;
using Refit;
using MudBlazor.Services;
using Marten;
using Weasel.Core;



Console.WriteLine("Program started");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var gitHubApi = RestService.For<IWeatherApi>("https://api.weatherapi.com/");
builder.Services.AddSingleton(gitHubApi);
builder.Services.AddMudServices();
builder.Services.AddSingleton<TcpConnection>();

// This is the absolute, simplest way to integrate Marten into your
// .NET application with Marten's default configuration
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
});

var app = builder.Build();

var weatherApi = app.Services.GetService<IWeatherApi>();

var store = app.Services.GetService<IDocumentStore>();


// Default cityName
LocationValidator.cityName = "Gais AR";

var _weatherGetter = new WeatherGetter(weatherApi, store);


// Initialize a new TcpConnection and start it in the background on another thread
var tcp = new TcpConnection();

var thread = new ThreadStart(tcp.TcpConnectionInit);
var backgroundThread = new Thread(thread);
backgroundThread.Start();


// Start the fetching process
_weatherGetter.setTimers();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
