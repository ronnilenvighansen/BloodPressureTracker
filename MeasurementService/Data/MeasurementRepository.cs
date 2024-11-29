using Microsoft.EntityFrameworkCore;
using MeasurementService.Data;
using MeasurementService.Models;

public class MeasurementRepository
{
    private readonly MeasurementDbContext _context;

    public MeasurementRepository(MeasurementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Measurement>> GetAllMeasurementsAsync()
    {
        return await _context.Measurements.ToListAsync();
    }

    public async Task<Measurement> GetMeasurementByIdAsync(int id)
    {
        return await _context.Measurements
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AddMeasurementAsync(Measurement measurement)
    {
        _context.Measurements.Add(measurement);
        await _context.SaveChangesAsync();
    }
//check redundancy put method
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
