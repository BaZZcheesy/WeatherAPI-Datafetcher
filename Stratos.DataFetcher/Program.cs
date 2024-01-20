using Refit;
using MudBlazor.Services;
using Marten;
using Stratos.DataFetcher.Api;
using Stratos.DataFetcher.Processes;
using Weasel.Core;
using Stratos.DataFetcher.Pages;

Console.WriteLine("Program started");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var gitHubApi = RestService.For<IWeatherApi>("https://api.weatherapi.com/");
builder.Services.AddSingleton(gitHubApi);
builder.Services.AddMudServices();

Console.WriteLine("services added");

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

Console.WriteLine("Marten added");

var app = builder.Build();

var weatherApi = app.Services.GetService<IWeatherApi>();

var store = app.Services.GetService<IDocumentStore>();

// Dependency Injection with constructor
var _weatherGetter = new WeatherGetter(weatherApi, store);

Console.WriteLine("DI completed");

// Start the fetching process
_weatherGetter.setTimers();

Console.WriteLine("Starting fetching process");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Console.WriteLine("app.environment zeugs lol");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Console.WriteLine("Program complete");

app.Run();
