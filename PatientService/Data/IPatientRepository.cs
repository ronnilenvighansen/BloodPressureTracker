using PatientService.Models;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllPatientsAsync();
    Task<Patient?> GetPatientAsync(string ssn);
    Task AddPatientAsync(Patient patient);
    Task UpdatePatientAsync(Patient updatedPatient);
    Task DeletePatientAsync(string ssn);
}
