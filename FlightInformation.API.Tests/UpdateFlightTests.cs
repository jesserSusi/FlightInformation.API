using FlightInformation.API.Controllers;
using FlightInformation.API.Entities;
using FlightInformation.API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FlightInformationAPI.Tests
{
    [TestFixture]
    public class UpdateFlightTests
    {
        private readonly IFlightService _flightService;
        private readonly ILogger<FlightsController> _logger;
        private FlightsController _flightsController;
        private readonly Flight _flightToUpdate;
        
        public UpdateFlightTests()
        {
            _flightService = Substitute.For<IFlightService>(); // mocking IFlightService
            _logger = Substitute.For<ILogger<FlightsController>>(); // mocking ILogger
            _flightsController = new FlightsController(_flightService, _logger);
            
            _flightToUpdate = new Flight()
            {
                Id = 1,
                FlightNumber = "JE150",
                Airline = "JE150",
                DepartureAirport = "WLG",
                ArrivalAirport = "SIN",
                DepartureTime = DateTime.Parse("2025-06-24T23:00:00"),
                ArrivalTime = DateTime.Parse("2025-06-25T11:00:00"),
                Status = FlightStatus.InAir
            };
        }

        [Test]
        public async Task UpdateFlight_Should_Return_Updated_Flight_When_Successful()
        {
            // Arrange
            _flightService.UpdateFlight(Arg.Any<Flight>()).Returns(_flightToUpdate);
            
            // Act
            var result = await _flightsController.UpdateFlight(_flightToUpdate.Id, _flightToUpdate);
            var updatedFlightResult = (result.Result as OkObjectResult).Value as Flight;
            
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            Assert.AreEqual(updatedFlightResult.FlightNumber, _flightToUpdate.FlightNumber);
            Assert.AreEqual(updatedFlightResult.Airline, _flightToUpdate.Airline);
        }
        
        [Test]
        public async Task GetFlightById_Should_Return_NotFoundObjectResult_When_Id_Mismatched()
        {
            // Arrange
            int flightId = 2;
            
            // Act
            var result = await _flightsController.UpdateFlight(flightId, _flightToUpdate);
            var badResult = result.Result as NotFoundObjectResult;
            
            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            Assert.AreEqual("Flight 'id' mismatched", badResult.Value);
        }

        [Test]
        public async Task GetFlightById_Should_Return_NotFoundObjectResult_When_NotFound()
        {
            // Arrange
           var flightId = 2;
           _flightService.UpdateFlight(Arg.Any<Flight>()).Returns((Flight)null);
           
           // Act
           var result = await _flightsController.UpdateFlight(flightId, _flightToUpdate);
           
           // Assert
           Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }

        [Test]
        public async Task UpdateFlight_Should_Return_BadRequestObjectResult_When_Flight_Details_Are_Invalid()
        {
            // Arrange
            var flightWithIssues = new Flight()
            {
                Id = 1,
                FlightNumber = null,
                Airline = "",
                DepartureAirport = "C2C",
                ArrivalAirport = "SYD3",
                DepartureTime = DateTime.Parse("2025-06-24T23:00:00"),
                ArrivalTime = DateTime.Parse("2025-06-25T11:00:00"),
                Status = FlightStatus.Scheduled
            };
            
            // Act
            var result = await _flightsController.UpdateFlight(flightWithIssues.Id, flightWithIssues);
            
            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }
    }
}