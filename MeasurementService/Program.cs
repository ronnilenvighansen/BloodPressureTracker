using Microsoft.EntityFrameworkCore;
using MeasurementService.Data;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.MeasurementService.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.MeasurementService.Development.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<MeasurementDbContext>(options =>
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

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MeasurementDbContext>();

    if (!dbContext.Database.GetPendingMigrations().Any())
    {
        Console.WriteLine("No pending migrations. Skipping migration.");
    }
    else
    {
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations applied.");
    }
}

app.UseAuthorization();
app.MapControllers();

app.Run();