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

    public async Task<IEnumerable<Measurement>> GetMeasurementsByPatientAsync(string patientSSN)
    {
        return await _context.Measurements.Where(m => m.PatientSSN == patientSSN).ToListAsync();
    }

    public async Task<Measurement> GetMeasurementByIdAsync(int id)
    {
        // Retrieve the measurement without tracking it for update operations
        var measurement = await _context.Measurements
            .AsNoTracking()  // Ensure no tracking for this query
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
        // Check if the measurement exists in the database
        var existingMeasurement = await _context.Measurements
            .FirstOrDefaultAsync(m => m.Id == updatedMeasurement.Id);

        if (existingMeasurement == null)
        {
            throw new KeyNotFoundException("Measurement not found.");
        }

        // Attach the updated entity and mark it as modified
        var entry = _context.Entry(updatedMeasurement);
        
        if (entry.State == EntityState.Detached)
        {
            // If the entity is not being tracked, attach it
            _context.Measurements.Attach(updatedMeasurement);
        }
        
        // Ensure the state is marked as modified so EF Core knows to update it
        entry.State = EntityState.Modified;

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
