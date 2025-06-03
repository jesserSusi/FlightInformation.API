using FlightInformation.API.Controllers;
using FlightInformation.API.Entities;
using FlightInformation.API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FlightInformationAPI.Tests
{
    [TestFixture]
    public class GetListFlightTests
    {
        private readonly IFlightService _flightService;
        private readonly ILogger<FlightsController> _logger;
        private FlightsController _flightsController;
        private readonly List<Flight> _flights;

        public GetListFlightTests()
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
        public async Task GetAll_Should_Return_List_Of_Flights()
        {
            // Arrange
            _flightService.GetFlights().Returns(_flights);

            // Act
            var result = await _flightsController.GetFlights();
            var getAllResult = (result.Result as OkObjectResult).Value as List<Flight>;

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            Assert.IsInstanceOf<List<Flight>>((result.Result as OkObjectResult).Value);
            Assert.True(getAllResult.Count == 3);
        }

        [Test]
        public async Task GetFlightById_Should_Return_OkResult_With_Flight()
        {
            // Arrange
            var flightId = 1;
            var flight = _flights.FirstOrDefault(f => f.Id == flightId);
            _flightService.GetFlight(flightId).Returns(flight);

            // Act
            var result = await _flightsController.GetFlight(flightId);
            var getResult = (result.Result as OkObjectResult).Value as Flight;

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            Assert.IsInstanceOf<Flight>((result.Result as OkObjectResult).Value);
            Assert.AreEqual(flightId, getResult.Id);
        }

        [Test]
        public async Task GetFlightById_Should_Return_NotFound_When_Flight_Does_Not_Exist()
        {
            // Arrange
            var flightId = 1;
            
            // Act
            var result = await _flightsController.GetFlight(flightId);
            
            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }
    }
}