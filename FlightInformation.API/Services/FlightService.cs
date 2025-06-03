using System.Linq.Expressions;
using FlightInformation.API.Data;
using FlightInformation.API.Entities;
using FlightInformation.API.Helpers;
using FlightInformation.API.Interface;
using Microsoft.EntityFrameworkCore;

namespace FlightInformation.API.Services
{
    public class FlightService : IFlightService
    {
        private readonly FlightDbContext _dbContext;

        public FlightService(FlightDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Flight>> GetFlights()
        {
            return await _dbContext.Flights
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task<Flight> GetFlight(int id)
        {
            return await _dbContext.Flights
                .AsNoTracking()
                .FirstOrDefaultAsync(flight => flight.Id == id);
        }

        public async Task<Flight> AddFlight(Flight flight)
        {
            var newFlight = await _dbContext.Flights.AddAsync(flight);
            await _dbContext.SaveChangesAsync();
            return newFlight.Entity;
        }

        public async Task<Flight> UpdateFlight(Flight flightToUpdate)
        {
            var updatedFlight = await _dbContext.Flights
                .FirstOrDefaultAsync(flight => flight.Id == flightToUpdate.Id);

            if (updatedFlight != null)
            {
                updatedFlight.FlightNumber = flightToUpdate.FlightNumber;
                updatedFlight.Airline = flightToUpdate.Airline;
                updatedFlight.ArrivalAirport = flightToUpdate.ArrivalAirport;
                updatedFlight.DepartureAirport = flightToUpdate.DepartureAirport;
                updatedFlight.ArrivalTime = flightToUpdate.ArrivalTime;
                updatedFlight.DepartureTime = flightToUpdate.DepartureTime;
                updatedFlight.Status = flightToUpdate.Status;

                await _dbContext.SaveChangesAsync();
                return updatedFlight;
            }

            return null;
        }

        public async Task DeleteFlight(Flight flight)
        {
            _dbContext.Flights.Remove(flight);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Flight>> SearchFlights(string? airline, string? airport, 
            DateTime? startDate, DateTime? endDate)
        {
            var queryConditions = new FlightQueryConditions(airline, airport, startDate, endDate);

            // Create a list of LINQ expressions based on the criteria
            var queryBuilder = new List<Expression<Func<Flight, bool>>>();

            if (!string.IsNullOrEmpty(airline))
            {
                queryBuilder.Add(queryConditions.BelongsToAirline);
            }

            if (!string.IsNullOrEmpty(airport))
            {
                queryBuilder.Add(queryConditions.BelongsToAirport);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                queryBuilder.Add(queryConditions.FallsInDateRange);
            }
            else if (!endDate.HasValue && startDate.HasValue)
            {
                queryBuilder.Add(queryConditions.BelongsToDepartureDate);
            }
            else if (!startDate.HasValue && endDate.HasValue)
            {
                queryBuilder.Add(queryConditions.BelongsToArrivalDate);
            }

            return await _dbContext.Flights
                .AsQueryable()
                .AsNoTracking()
                .ApplyAllFilters(queryBuilder)
                .ToListAsync();
        }
    }
}