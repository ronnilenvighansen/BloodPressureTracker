using Microsoft.EntityFrameworkCore;
using MeasurementService.Data;
using Unleash;
using Polly;
using MeasurementService.Services;

var builder = WebApplication.CreateBuilder(args);

var retryPolicy = Policy.Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .RetryAsync(3);

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));

builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(retryPolicy);
builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(timeoutPolicy);

builder.Services.AddHttpClient<SSNValidationService>(client =>
{
    var baseAddress = builder.Configuration["PatientService:BaseAddress"];
    client.BaseAddress = new Uri(baseAddress);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler())
.AddHttpMessageHandler(sp =>
{
    var originalRetryPolicy = sp.GetRequiredService<IAsyncPolicy<HttpResponseMessage>>();
    var timeoutPolicy = sp.GetRequiredService<IAsyncPolicy<HttpResponseMessage>>();

    var loggingRetryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .RetryAsync(3, onRetry: (outcome, retryNumber, context) =>
        {
            Console.WriteLine($"Retry {retryNumber} for request. " +
                              $"StatusCode: {outcome.Result?.StatusCode}, " +
                              $"Exception: {outcome.Exception?.Message}");
        });

    var combinedRetryPolicy = originalRetryPolicy.WrapAsync(loggingRetryPolicy);

    return new PolicyHandler(combinedRetryPolicy, timeoutPolicy);
});

builder.Services.AddDbContext<MeasurementDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddScoped<IMeasurementRepository, MeasurementRepository>();

var unleashUrl = builder.Configuration["UNLEASH_URL"];
var unleashApiToken = builder.Configuration["UNLEASH_API_TOKEN"];

var settings = new UnleashSettings()
{
    AppName = "measurement-service",
    UnleashApi = new Uri(unleashUrl),
    CustomHttpHeaders = new Dictionary<string, string>
    {
        { "Authorization", unleashApiToken }
    }
};

var unleash = new DefaultUnleash(settings);

builder.Services.AddSingleton<IUnleash>(unleash);

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MeasurementDbContext>();

    try
    {
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("No pending migrations.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
    }
}

app.UseAuthorization();
app.MapControllers();

app.Run();