using Microsoft.AspNetCore.Mvc;
using MeasurementService.Models;
using Unleash;
using MeasurementService.Services;

[ApiController]
[Route("api/[controller]")]
public class MeasurementController : ControllerBase
{
    private readonly SSNValidationService _ssnValidationService;
    private readonly MeasurementRepository _repository;
    private readonly IUnleash _unleash;

    public MeasurementController(SSNValidationService ssnValidationService, MeasurementRepository repository, IUnleash unleash)
    {
        _ssnValidationService = ssnValidationService;
        _repository = repository;
        _unleash = unleash;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Measurement>>> GetAllMeasurements()
    {
        if (!_unleash.IsEnabled("measurement-service.get-all"))
        {
            return StatusCode(503, "Feature disabled.");
        }

        var measurements = await _repository.GetAllMeasurementsAsync();
        return Ok(measurements);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeasurement(int id)
    {
        if (!_unleash.IsEnabled("measurement-service.get-by-id"))
        {
            return StatusCode(503, "Feature disabled.");
        }

        var measurement = await _repository.GetMeasurementByIdAsync(id);
        if (measurement == null)
        {
            return NotFound();
        }

        return Ok(measurement);
    }

    [HttpPost]
    public async Task<IActionResult> AddMeasurement([FromBody] Measurement measurement)
    {
        if (!_unleash.IsEnabled("measurement-service.add"))
        {
            return StatusCode(503, "Feature disabled.");
        }

        // Validate SSN before proceeding
        var isValidSSN = await _ssnValidationService.ValidateSSNAsync(measurement.PatientSSN);
        if (!isValidSSN)
        {
            return BadRequest("Invalid SSN.");
        }
        
        // Add the measurement to the repository directly
        await _repository.AddMeasurementAsync(measurement);

        // Optionally, you can return the measurement data directly with a 201 Created status
        return CreatedAtAction(nameof(GetMeasurement), new { id = measurement.Id }, measurement);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeasurement(int id, Measurement updatedMeasurement)
    {
        if (!_unleash.IsEnabled("measurement-service.update"))
        {
            return StatusCode(503, "Feature disabled.");
        }

        if (id != updatedMeasurement.Id)
            return BadRequest("ID mismatch.");

        try
        {
            await _repository.UpdateMeasurementAsync(updatedMeasurement);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeasurement(int id)
    {
        if (!_unleash.IsEnabled("measurement-service.delete"))
        {
            return StatusCode(503, "Feature disabled.");
        }

        var existingMeasurement = await _repository.GetAllMeasurementsAsync();
        if (existingMeasurement == null)
        {
            return NotFound();
        }

        await _repository.DeleteMeasurementAsync(id);
        return NoContent();
    }
}
