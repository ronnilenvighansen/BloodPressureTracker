[ApiController]
[Route("api/measurements")]
public class MeasurementController : ControllerBase
{
    private readonly MeasurementRepository _repository;

    public MeasurementController(MeasurementRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> AddMeasurement([FromBody] Measurement measurement)
    {
        await _repository.AddMeasurementAsync(measurement);
        return CreatedAtAction(nameof(GetMeasurementsByPatient), new { ssn = measurement.PatientSSN }, measurement);
    }

    [HttpGet("patient/{ssn}")]
    public async Task<IActionResult> GetMeasurementsByPatient(string ssn)
    {
        var measurements = await _repository.GetMeasurementsByPatientAsync(ssn);
        return Ok(measurements);
    }
}
