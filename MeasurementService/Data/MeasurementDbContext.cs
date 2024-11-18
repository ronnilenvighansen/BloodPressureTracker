using Microsoft.EntityFrameworkCore;
using MeasurementService.Models;

namespace MeasurementService.Data
{
    public class MeasurementDbContext : DbContext
    {
        public MeasurementDbContext(DbContextOptions<MeasurementDbContext> options)
            : base(options) { }

        public DbSet<Measurement> Measurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Measurement>(entity =>
            {
                entity.HasKey(m => m.Id);
                
                entity.Property(m => m.PatientSSN)
                    .IsRequired();
            });
        }

    }
}
