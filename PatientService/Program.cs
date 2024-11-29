using Microsoft.EntityFrameworkCore;
using PatientService.Data;
using Unleash;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddScoped<PatientRepository>();

var unleashUrl = builder.Configuration["UNLEASH_URL"];
var unleashApiToken = builder.Configuration["UNLEASH_API_TOKEN"];

var settings = new UnleashSettings()
{
    AppName = "patient-service",
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
    var context = scope.ServiceProvider.GetRequiredService<PatientDbContext>();

    try
    {
        // Check if migrations are needed
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
