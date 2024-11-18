using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models.Measurement;

public class MeasurementRepository
{
    private readonly SharedDbContext _context;

    public MeasurementRepository(SharedDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Measurement>> GetAllMeasurementsAsync()
    {
        return await _context.Measurements.ToListAsync();
    }

    public async Task<IEnumerable<Measurement>> GetMeasurementsByPatientAsync(string patientSSN)
    {
        return await _context.Measurements.Where(m => m.PatientSSN == patientSSN).ToListAsync();
    }

    public async Task<Measurement> GetMeasurementByIdAsync(int id)
    {
        var measurement = await _context.Measurements
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        
        return measurement;
    }

    public async Task AddMeasurementAsync(Measurement measurement)
    {
        _context.Measurements.Add(measurement);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMeasurementAsync(Measurement updatedMeasurement)
    {
        var existingEntity = _context.ChangeTracker.Entries<Measurement>()
                                    .FirstOrDefault(e => e.Entity.Id == updatedMeasurement.Id);
        if (existingEntity != null)
        {
            _context.Entry(existingEntity.Entity).State = EntityState.Detached;
        }

        _context.Measurements.Attach(updatedMeasurement);
        _context.Entry(updatedMeasurement).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteMeasurementAsync(int id)
    {
        var measurement = await _context.Measurements.FindAsync(id);
        if (measurement != null)
        {
            _context.Measurements.Remove(measurement);
            await _context.SaveChangesAsync();
        }
    }
}
