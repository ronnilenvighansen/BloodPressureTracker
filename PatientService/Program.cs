using Microsoft.EntityFrameworkCore;
using Shared.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SharedDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddScoped<PatientRepository>();

builder.Services.AddControllers();

var app = builder.Build();

var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
var redisConnectionString = $"{redisHost}:6379";
var redis = ConnectionMultiplexer.Connect(redisConnectionString);
var db = redis.GetDatabase();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SharedDbContext>();

    var lockKey = "migrations-lock";
    var lockValue = Guid.NewGuid().ToString();
    var lockAcquired = db.LockTake(lockKey, lockValue, TimeSpan.FromMinutes(5));

    if (lockAcquired)
    {
        try
        {
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
        finally
        {
            db.LockRelease(lockKey, lockValue);
        }
    }
    else
    {
        Console.WriteLine("Could not acquire lock. Skipping migration.");
    }
}

app.UseAuthorization();
app.MapControllers();

app.Run();
