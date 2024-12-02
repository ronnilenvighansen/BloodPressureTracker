using Microsoft.AspNetCore.Mvc;
using Moq;
using PatientService.Models;
using Unleash;

namespace PatientService.Tests;

public class PatientControllerTests
{
    private Mock<IPatientRepository> _mockPatientRepository;
    private Mock<IUnleash> _mockUnleash;
    private PatientController _controller;

    public PatientControllerTests()
    {
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockUnleash = new Mock<IUnleash>();

        _controller = new PatientController(_mockPatientRepository.Object, _mockUnleash.Object);
    }

    [Fact]
    public async Task GetAllPatients_ReturnsOkResultWithPatients_WhenFeatureEnabled()
    {
        // Arrange
        var mockPatients = new List<Patient>
        {
            new Patient { SSN = "12345", Name = "John Doe", Mail = "john.doe@example.com" }
        };

        _mockUnleash.Setup(u => u.IsEnabled(It.IsAny<string>())).Returns(true);
        _mockPatientRepository.Setup(r => r.GetAllPatientsAsync()).ReturnsAsync(mockPatients);

        // Act
        var result = await _controller.GetAllPatients();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Patient>>>(result);

        // Verify the inner OkObjectResult
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Patient>>(okResult.Value);
        Assert.Equal(mockPatients.Count(), returnValue.Count());
    }
}

