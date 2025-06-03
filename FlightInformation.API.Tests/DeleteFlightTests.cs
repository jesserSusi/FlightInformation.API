using FlightInformation.API.Controllers;
using FlightInformation.API.Entities;
using FlightInformation.API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FlightInformationAPI.Tests
{
    [TestFixture]
    public class DeleteFlightTests
    {
        private readonly IFlightService _flightService;
        private readonly ILogger<FlightsController> _logger;
        private FlightsController _flightsController;
        private readonly List<Flight> _flights;

        public DeleteFlightTests()
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
        }

        [Test]
        public async Task DeleteFlight_Should_Return_NoContentResult()
        {
            // Arrange
            var flightId = 1;
            var flight = _flights.FirstOrDefault(x => x.Id == flightId);
            _flightService.GetFlight(flight.Id).Returns(flight);
            
            // Act
            var result = await _flightsController.DeleteFlight(flight.Id);
            
            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteFlight_Should_Return_NotFoundObjectResult_When_Flight_Does_Not_Exist()
        {
            // Arrange
            var flightId = 999;
            
            // Act
            var result = await _flightsController.DeleteFlight(flightId);
            
            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}