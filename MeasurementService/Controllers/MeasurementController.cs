using Microsoft.AspNetCore.Mvc;
using MeasurementService.Models;
using PatientService.Models;

[ApiController]
[Route("api/[controller]")]
public class MeasurementController : ControllerBase
{
    private readonly MeasurementRepository _repository;
    private readonly PatientServiceClient _patientServiceClient;

    public MeasurementController(MeasurementRepository repository, PatientServiceClient patientServiceClient)
    {
        _repository = repository;
        _patientServiceClient = patientServiceClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Measurement>>> GetAllMeasurements()
    {
        var measurements = await _repository.GetAllMeasurementsAsync();
        return Ok(measurements);
    }

    [HttpGet("{ssn}")]
    public async Task<IActionResult> GetMeasurementsByPatient(string ssn)
    {
        var measurements = await _repository.GetMeasurementsByPatientAsync(ssn);
        return Ok(measurements);
    }

    [HttpPost]
    public async Task<IActionResult> AddMeasurement([FromBody] Measurement measurement)
    {
        var patient = await _patientServiceClient.GetPatientBySSNAsync(measurement.PatientSSN);

        if (patient == null)
        {
            return NotFound("Patient not found");
        }

        await _repository.AddMeasurementAsync(measurement);
        return CreatedAtAction(nameof(GetMeasurementsByPatient), new { ssn = measurement.PatientSSN }, measurement);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeasurement(int id, [FromBody] Measurement updatedMeasurement)
    {
        if (id != updatedMeasurement.Id)
        {
            return BadRequest("ID in the URL and body do not match.");
        }

        var existingMeasurement = await _repository.GetMeasurementByIdAsync(id); // Fetch a single measurement
        if (existingMeasurement == null)
        {
            return NotFound();
        }

        await _repository.UpdateMeasurementAsync(updatedMeasurement);
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeasurement(int id)
    {
        var existingMeasurement = await _repository.GetAllMeasurementsAsync();
        if (existingMeasurement == null)
        {
            return NotFound();
        }

        await _repository.DeleteMeasurementAsync(id);
        return NoContent();
    }
}
