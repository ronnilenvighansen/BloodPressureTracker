using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Unleash;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<SharedDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

var patientServiceUrl = builder.Configuration.GetValue<string>("PatientService:BaseUrl") 
                        ?? "http://localhost:6000";

builder.Services.AddHttpClient<PatientServiceClient>(client =>
{
    client.BaseAddress = new Uri(patientServiceUrl);
});

builder.Services.AddScoped<MeasurementRepository>();

var settings = new UnleashSettings()
{
    AppName = "measurement-service",
    UnleashApi = new Uri("https://app.unleash-host.com/api"), // Replace with your Unleash server URL
    CustomHttpHeaders = new Dictionary<string, string>
    {
        { "Authorization", "<your-api-token>" } // Replace with your API token
    }
};

var unleash = new DefaultUnleash(settings);

// Register DefaultUnleash as a singleton service
builder.Services.AddSingleton<IUnleash>(unleash);

// Example of checking feature toggles
bool isFeatureEnabled = unleash.IsEnabled("measurement-service.get-all", false); // Fallback to false
bool isAnotherFeatureEnabled = unleash.IsEnabled("measurement-service.get-by-patient", true); // Fallback to true

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();