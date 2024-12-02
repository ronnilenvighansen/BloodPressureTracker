using Microsoft.AspNetCore.Mvc;
using Moq;
using PatientService.Models;
using Unleash;

namespace PatientService.Tests;

public class PatientControllerTests
{
    [Fact]
    public async Task GetAllPatients_ReturnsOkResultWithPatients_WhenFeatureEnabled()
    {
        // Arrange
        var mockRepository = new Mock<PatientRepository>(null);
        var mockUnleash = new Mock<IUnleash>();

        var samplePatients = new List<Patient>
        {
            new Patient { SSN = "123456-7890", Name = "John Doe", Mail = "john.doe@example.com" },
            new Patient { SSN = "987654-3210", Name = "Jane Smith", Mail = "jane.smith@example.com" }
        };

        mockRepository.Setup(repo => repo.GetAllPatientsAsync())
                        .ReturnsAsync(samplePatients);

        mockUnleash.Setup(unleash => unleash.IsEnabled("patient-service.get-all-patients"))
                    .Returns(true);

        var controller = new PatientController(mockRepository.Object, mockUnleash.Object);

        // Act
        var result = await controller.GetAllPatients();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var patients = Assert.IsAssignableFrom<IEnumerable<Patient>>(okResult.Value);
        Assert.Equal(2, patients.Count());
    }
}

