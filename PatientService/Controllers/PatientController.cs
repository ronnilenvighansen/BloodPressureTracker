[ApiController]
[Route("api/patients")]
public class PatientController : ControllerBase
{
    private readonly PatientRepository _repository;

    public PatientController(PatientRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> AddPatient([FromBody] Patient patient)
    {
        await _repository.AddPatientAsync(patient);
        return CreatedAtAction(nameof(GetPatient), new { ssn = patient.SSN }, patient);
    }

    [HttpGet("{ssn}")]
    public async Task<IActionResult> GetPatient(string ssn)
    {
        var patient = await _repository.GetPatientAsync(ssn);
        return patient == null ? NotFound() : Ok(patient);
    }
}
