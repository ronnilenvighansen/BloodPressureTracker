using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models.Patient;

public class PatientRepository
{
    private readonly SharedDbContext _context;

    public PatientRepository(SharedDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        return await _context.Patients.ToListAsync();
    }
    
    public async Task<Patient?> GetPatientAsync(string ssn)
    {
        return await _context.Patients.FindAsync(ssn);
    }

    public async Task AddPatientAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePatientAsync(Patient updatedPatient)
    {
        var existingPatient = await _context.Patients
            .FirstOrDefaultAsync(p => p.SSN == updatedPatient.SSN);

        if (existingPatient != null)
        {
            _context.Entry(existingPatient).State = EntityState.Detached;

            _context.Patients.Update(updatedPatient);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeletePatientAsync(string ssn)
    {
        var patient = await GetPatientAsync(ssn);
        if (patient != null)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
    }
}
