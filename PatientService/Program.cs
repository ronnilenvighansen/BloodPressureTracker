using Microsoft.EntityFrameworkCore;
using PatientService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddScoped<PatientRepository>();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PatientDbContext>();
    
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
