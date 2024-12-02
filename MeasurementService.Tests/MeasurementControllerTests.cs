using Microsoft.AspNetCore.Mvc;
using Moq;
using MeasurementService.Models;
using Unleash;
using MeasurementService.Services;
using Polly;

namespace MeasurementService.Tests;

public class MeasurementControllerTests
{
    [Fact]
    public async Task GetAllMeasurements_ReturnsOkResultWithMeasurements_WhenFeatureEnabled()
    {
        // Arrange
        var mockHttpClient = new Mock<HttpClient>(); // Mock HttpClient if needed
        var mockRetryPolicy = new Mock<IAsyncPolicy<HttpResponseMessage>>(); // Mock retry policy
        var mockTimeoutPolicy = new Mock<IAsyncPolicy<HttpResponseMessage>>(); // Mock timeout policy

        // Create an instance of SSNValidationService using the mocked dependencies
        var ssnValidationService = new SSNValidationService(
            mockHttpClient.Object, 
            mockRetryPolicy.Object, 
            mockTimeoutPolicy.Object
        );

        // Set up your repository mock
        var mockRepository = new Mock<IMeasurementRepository>();
        var mockUnleash = new Mock<IUnleash>();

        // Setup the measurements
        var sampleMeasurements = new List<Measurement>
        {
            new Measurement { Id = 1, Systolic = 120, Diastolic = 80, Date = DateTime.Now },
            new Measurement { Id = 2, Systolic = 130, Diastolic = 85, Date = DateTime.Now }
        };

        mockRepository.Setup(repo => repo.GetAllMeasurementsAsync())
                    .ReturnsAsync(sampleMeasurements);

        mockUnleash.Setup(unleash => unleash.IsEnabled("measurement-service.get-all"))
           .Returns(true);


        // Create the controller instance
        var controller = new MeasurementController(
            ssnValidationService, // Pass the mocked SSNValidationService
            mockRepository.Object, 
            mockUnleash.Object
        );

        // Act
        var result = await controller.GetAllMeasurements();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var measurements = Assert.IsAssignableFrom<IEnumerable<Measurement>>(okResult.Value);
        Assert.Equal(2, measurements.Count());
    }
}

