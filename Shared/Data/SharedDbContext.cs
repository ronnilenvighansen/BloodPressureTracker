using Microsoft.EntityFrameworkCore;
using Shared.Models.Measurement;
using Shared.Models.Patient;

namespace Shared.Data
{
    public class SharedDbContext : DbContext
    {
        public SharedDbContext(DbContextOptions<SharedDbContext> options)
            : base(options)
        { }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Measurement> Measurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.SSN);

            modelBuilder.Entity<Measurement>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.PatientSSN)
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
