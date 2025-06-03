using FlightInformation.API.Controllers;
using FlightInformation.API.Entities;
using FlightInformation.API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FlightInformationAPI.Tests
{
    [TestFixture]
    public class CreateFlightTests
    {
        private readonly IFlightService _flightService;
        private readonly ILogger<FlightsController> _logger;
        private FlightsController _flightsController;
        private readonly List<Flight> _flights;
        private readonly Flight _flightToCreate;
        
        public CreateFlightTests()
        {
            _flightService = Substitute.For<IFlightService>(); // mocking IFlightService
            _logger = Substitute.For<ILogger<FlightsController>>(); // mocking ILogger
            _flightsController = new FlightsController(_flightService, _logger);
            
            _flights = new List<Flight>()
            {
                new Flight()
                {
                    Id = 1,
                    FlightNumber = "AI101",
                    Airline = "Air New Zealand",
                    DepartureAirport = "CHC",
                    ArrivalAirport = "SYD",
                    DepartureTime = DateTime.Parse("2025-06-24T23:00:00"),
                    ArrivalTime = DateTime.Parse("2025-06-25T11:00:00"),
                    Status = FlightStatus.Cancelled
                },
                new Flight()
                {
                    Id = 2,
                    FlightNumber = "VI102",
                    Airline = "Virgin Australia",
                    DepartureAirport = "AKL",
                    ArrivalAirport = "DXB",
                    DepartureTime = DateTime.Parse("2025-06-20T11:00:00"),
                    ArrivalTime = DateTime.Parse("2025-06-20T17:00:00"),
                    Status = FlightStatus.Delayed
                },
                new Flight()
                {
                    Id = 3,
                    FlightNumber = "QA103",
                    Airline = "Qantas",
                    DepartureAirport = "NPE",
                    ArrivalAirport = "FJI",
                    DepartureTime = DateTime.Parse("2025-06-18T05:00:00"),
                    ArrivalTime = DateTime.Parse("2025-06-18T11:00:00"),
                    Status = FlightStatus.Landed
                }
            };
            
            _flightToCreate = new Flight()
            {
                Id = 1,
                FlightNumber = "AI101",
                Airline = "Air New Zealand",
                DepartureAirport = "CHC",
                ArrivalAirport = "SYD",
                DepartureTime = DateTime.Parse("2025-06-24T23:00:00"),
                ArrivalTime = DateTime.Parse("2025-06-25T11:00:00"),
                Status = FlightStatus.Scheduled
            };
        }

        [Test]
        public async Task CreateFlight_Returns_CreatedAtActionResult_When_Flight_Added_Successfully()
        {
            // Arrange
            var flight = _flights.FirstOrDefault(x => x.FlightNumber == "AI101" && x.Airline == "Air New Zealand");
            _flightService.AddFlight(Arg.Any<Flight>()).Returns(flight);
            
            // Act
            var result = await _flightsController.CreateFlight(_flightToCreate);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var newFlightResult = createdAtActionResult.Value as Flight;
            
            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            Assert.IsInstanceOf<Flight>(newFlightResult);
            Assert.AreEqual(nameof(_flightsController.GetFlight), createdAtActionResult.ActionName);
            Assert.AreEqual(1, createdAtActionResult.RouteValues["id"]);
            Assert.AreEqual(flight.Id, _flightToCreate.Id);
        }

        [Test]
        public async Task CreateFlight_Returns_ObjectResult_When_Service_Throws_Exception()
        {
            // Arrange
            _flightService.AddFlight(Arg.Any<Flight>()).Throws<Exception>();
            
            // Act
            var result = await _flightsController.CreateFlight(_flightToCreate);
            var errorResult = result.Result as ObjectResult;
            
            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.AreEqual(500, errorResult.StatusCode);
        }

        [Test] 
        public async Task CreateFlight_Returns_BadRequestObjectResult_When_Flight_Details_Are_Invalid()
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
            var result = await _flightsController.CreateFlight(flightWithIssues);
            
            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }
    }
}