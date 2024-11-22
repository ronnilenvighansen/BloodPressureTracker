using Microsoft.AspNetCore.Mvc;
using Shared.Models.Patient;
using Unleash;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly PatientRepository _repository;
    private readonly IUnleash _unleash;

    public PatientController(PatientRepository repository, IUnleash unleash)
    {
        _repository = repository;
        _unleash = unleash;
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
        if (!_unleash.IsEnabled("patient-service.get-patient"))
        {
            return StatusCode(503, "Feature disabled.");
        }

        var patient = await _repository.GetPatientAsync(ssn);
        return patient == null ? NotFound() : Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> AddPatient([FromBody] Patient patient)
    {
        if (!_unleash.IsEnabled("patient-service.add-patient"))
        {
            return StatusCode(503, "Feature disabled.");
        }

        await _repository.AddPatientAsync(patient);
        return CreatedAtAction(nameof(GetPatient), new { ssn = patient.SSN }, patient);
    }

    [HttpPut("{ssn}")]
    public async Task<IActionResult> UpdatePatient(string ssn, [FromBody] Patient updatedPatient)
    {
        if (!_unleash.IsEnabled("patient-service.update-patient"))
        {
            return StatusCode(503, "Feature disabled.");
        }

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
        if (!_unleash.IsEnabled("patient-service.delete-patient"))
        {
            return StatusCode(503, "Feature disabled.");
        }

        var existingPatient = await _repository.GetPatientAsync(ssn);
        if (existingPatient == null)
        {
            return NotFound();
        }

        await _repository.DeletePatientAsync(ssn);
        return NoContent();
    }
}
