using Microsoft.AspNetCore.Mvc;
using PatientService.Models;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly PatientRepository _repository;

    public PatientController(PatientRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
    {
        var patients = await _repository.GetAllPatientsAsync();
        return Ok(patients);
    }

    [HttpGet("{ssn}")]
    public async Task<IActionResult> GetPatient(string ssn)
    {
        var patient = await _repository.GetPatientAsync(ssn);
        return patient == null ? NotFound() : Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> AddPatient([FromBody] Patient patient)
    {
        await _repository.AddPatientAsync(patient);
        return CreatedAtAction(nameof(GetPatient), new { ssn = patient.SSN }, patient);
    }

    [HttpPut("{ssn}")]
    public async Task<IActionResult> UpdatePatient(string ssn, [FromBody] Patient updatedPatient)
    {
        if (ssn != updatedPatient.SSN)
        {
            return BadRequest("SSN in the URL and body do not match.");
        }

        var existingPatient = await _repository.GetPatientAsync(ssn);
        if (existingPatient == null)
        {
            return NotFound();
        }

        await _repository.UpdatePatientAsync(updatedPatient);
        return NoContent();
    }

    [HttpDelete("{ssn}")]
    public async Task<IActionResult> DeletePatient(string ssn)
    {
        var existingPatient = await _repository.GetPatientAsync(ssn);
        if (existingPatient == null)
        {
            return NotFound();
        }

        await _repository.DeletePatientAsync(ssn);
        return NoContent();
    }
}
