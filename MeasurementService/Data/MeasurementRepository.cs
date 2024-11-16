public class MeasurementRepository
{
    private readonly DbContext _context;

    public MeasurementRepository(DbContext context)
    {
        _context = context;
    }

    public async Task AddMeasurementAsync(Measurement measurement)
    {
        _context.Measurements.Add(measurement);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Measurement>> GetMeasurementsByPatientAsync(string patientSSN)
    {
        return await _context.Measurements.Where(m => m.PatientSSN == patientSSN).ToListAsync();
    }
}
