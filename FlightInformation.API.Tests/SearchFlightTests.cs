using System.Text;
using FlightInformation.API.Controllers;
using FlightInformation.API.Entities;
using FlightInformation.API.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FlightInformationAPI.Tests
{
    [TestFixture]
    public class SearchFlightTests
    {
        private readonly IFlightService _flightService;
        private readonly ILogger<FlightsController> _logger;
        private FlightsController _flightsController;
        private readonly List<Flight> _flights;

        public SearchFlightTests()
        {
            _flightService = Substitute.For<IFlightService>(); // mocking IFlightService
            _logger = Substitute.For<ILogger<FlightsController>>(); // mocking ILogger
            _flightsController = new FlightsController(_flightService, _logger)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

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
        public async Task SearchFlights_Should_Return_Results_Based_On_The_Criteria()
        {
            // Arrange
            var airline = "Qantas";
            var flight = _flights.Where(x => x.Airline == airline);
            _flightsController.HttpContext.Request.QueryString = new QueryString($"?airline={airline}");
            _flightService.SearchFlights(airline, null, null, null).Returns(flight);
            
            // Act
            var result = await _flightsController.SearchFlights(airline, null, null, null);
            
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [TestCase("Air New Zealand", null, null, null)]
        [TestCase(null, "CHC", null, null)]
        [TestCase(null, null, "2025-06-24T23:00:00", "2025-06-20T17:00:00")]
        public async Task SearchFlights_Should_Return_Results_Based_On_The_Criteria(string? airline, string? airport,
            DateTime? startDate, DateTime? endDate)
        {
            // Arrange
            var flightId = 1;
            var queryString = new StringBuilder();
            queryString.Append(!string.IsNullOrEmpty(airline) ? $"?airline={airline}" : null);
            queryString.Append(!string.IsNullOrEmpty(airport) ? $"?airport={airport}" : null);
            queryString.Append(!string.IsNullOrEmpty(startDate.ToString()) && !string.IsNullOrEmpty(endDate.ToString())
                ? $"?startDate={startDate}&endDate={endDate}"
                : null);
            var flight = _flights.Where(x => x.Id == flightId);
            _flightsController.HttpContext.Request.QueryString = new QueryString(queryString.ToString());
            _flightService.SearchFlights(airline, null, null, null).Returns(flight);

            // Act
            var result = await _flightsController.SearchFlights(airline, null, null, null);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task SearchFlights_Should_Return_NotFoundObjectResult_When_No_Search_Criteria_Provided()
        {
            // Arrange
            _flightsController.HttpContext.Request.QueryString = new QueryString("");
            _flightService.SearchFlights(null, null, null, null).Returns((List<Flight>)null);
            
            // Act
            var result = await _flightsController.SearchFlights(null, null, null, null);
            
            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }
    }
}