using MeasurementService.Models;

public interface IMeasurementRepository
{
    Task<IEnumerable<Measurement>> GetAllMeasurementsAsync();
    Task<Measurement> GetMeasurementByIdAsync(int id);
    Task AddMeasurementAsync(Measurement measurement);
    Task UpdateMeasurementAsync(Measurement updatedMeasurement);
    Task DeleteMeasurementAsync(int id);
}
