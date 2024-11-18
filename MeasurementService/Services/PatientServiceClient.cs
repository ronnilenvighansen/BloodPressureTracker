using PatientService.Models;
using System.Text.Json;

public class PatientServiceClient
{
    private readonly HttpClient _httpClient;

    public PatientServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Patient> GetPatientBySSNAsync(string ssn)
    {
        var response = await _httpClient.GetAsync($"api/patient/{ssn}");

        if (response.IsSuccessStatusCode)
        {
            var patient = await ReadAsJsonAsync<Patient>(response);
            return patient;
        }

        return null; // Patient not found or some other error
    }

    private async Task<T> ReadAsJsonAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content);
    }
}
