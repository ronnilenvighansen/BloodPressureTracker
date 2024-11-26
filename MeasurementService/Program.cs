using Microsoft.EntityFrameworkCore;
using MeasurementService.Data;
using Unleash;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeasurementDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddScoped<MeasurementRepository>();

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
    context.Database.Migrate();
}


app.UseAuthorization();
app.MapControllers();

app.Run();