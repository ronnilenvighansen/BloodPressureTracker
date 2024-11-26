using Microsoft.AspNetCore.Mvc;
using MeasurementService.Models;
using Unleash;

[ApiController]
[Route("api/[controller]")]
public class MeasurementController : ControllerBase
{
    private readonly MeasurementRepository _repository;
    private readonly IUnleash _unleash;

    public MeasurementController(
        MeasurementRepository repository, 
        IUnleash unleash)
    {
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

        await _repository.AddMeasurementAsync(measurement);
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
