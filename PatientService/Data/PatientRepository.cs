public class PatientRepository
{
    private readonly DbContext _context;

    public PatientRepository(DbContext context)
    {
        _context = context;
    }

    public async Task AddPatientAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
    }

    public async Task<Patient?> GetPatientAsync(string ssn)
    {
        return await _context.Patients.FindAsync(ssn);
    }
}
