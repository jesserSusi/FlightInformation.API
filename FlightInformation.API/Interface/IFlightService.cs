using FlightInformation.API.Entities;

namespace FlightInformation.API.Interface;

public interface IFlightService
{
    Task<IEnumerable<Flight>> GetFlights();

    Task<Flight> GetFlight(int id);

    Task<Flight> AddFlight(Flight flight);

    Task<Flight> UpdateFlight(Flight flight);

    Task DeleteFlight(Flight flight);

    Task<IEnumerable<Flight>> SearchFlights(string? airline, string? airport, DateTime? startDate, DateTime? endDate);
}