using Microsoft.EntityFrameworkCore;
using MeasurementService.Models;

namespace MeasurementService.Data
{
    public class MeasurementDbContext : DbContext
    {
        public MeasurementDbContext(DbContextOptions<MeasurementDbContext> options)
            : base(options)
        { }

        public DbSet<Measurement> Measurements { get; set; }
    }
}
