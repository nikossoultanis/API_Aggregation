using API_Aggregation;
using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using API_Aggregation.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<OpenWeatherMapConfig>(builder.Configuration.GetSection("OpenWeatherMap"));
builder.Services.Configure<TwitterConfig>(builder.Configuration.GetSection("Twitter"));
builder.Services.Configure<NewsConfig>(builder.Configuration.GetSection("News"));
builder.Services.Configure<SpotifyConfig>(builder.Configuration.GetSection("Spotify"));
//builder.Services.Configure<GitHubConfig>(builder.Configuration.GetSection("GitHub"));

// We registers the Services as the implementation of the Interface of the Service with a scoped lifetime.
// Done for every Service. example OpenWeather, News etc.
// provides loose coupling, and increased testability
builder.Services.AddScoped<APICaching>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<RequestStatisticsService>();
builder.Services.AddScoped<IOpenWeatherMapService, OpenWeatherMapService>();
builder.Services.AddScoped<ITwitterService, TwitterService>();


builder.Services.AddScoped<INewsService, NewsService>();

//builder.Services.AddScoped<ISpotifyService, SpotifyService>();
builder.Services.AddScoped<ISpotifyService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var cacheUtility = sp.GetRequiredService<APICaching>();
    var config = builder.Configuration.GetSection("Spotify");
    var stats = sp.GetRequiredService<RequestStatisticsService>();
    return new SpotifyService(httpClient, config.GetSection("ClientID").Value.ToString(), config.GetSection("ClientIDSecret").Value.ToString(), cacheUtility, stats);
});

//builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICountryService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var cacheUtility = sp.GetRequiredService<APICaching>();
    var config = builder.Configuration.GetSection("Country");
    var stats = sp.GetRequiredService<RequestStatisticsService>();
    return new CountryService(httpClient, cacheUtility, stats);
});
builder.Services.AddScoped<IAggregationService, AggregationService>();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Aggregation Service", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI();

app.Run();
