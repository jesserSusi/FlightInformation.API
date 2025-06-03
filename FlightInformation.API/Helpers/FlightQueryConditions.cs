using System.Linq.Expressions;
using FlightInformation.API.Entities;

namespace FlightInformation.API.Helpers;

/// <summary>
/// Used for the search API and for creating the LINQ expressions.
/// </summary>
public class FlightQueryConditions
{
    private readonly string? _airline;
    private readonly string? _airport;
    private readonly DateTime? _startDate;
    private readonly DateTime? _endDate;
    
    public FlightQueryConditions(string? airline, string? airport, DateTime? startDate, DateTime? endDate)
    {
        _airline = airline;
        _airport = airport;
        _startDate = startDate;
        _endDate = endDate;
    }

    public Expression<Func<Flight, bool>> BelongsToAirline =>
        x => x.Airline.ToLower() == Helper.ToLower(_airline);

    public Expression<Func<Flight, bool>> BelongsToAirport =>
        x => x.DepartureAirport.ToLower() == Helper.ToLower(_airport) ||
             x.ArrivalAirport.ToLower() == Helper.ToLower(_airport);
    
    public Expression<Func<Flight, bool>> BelongsToArrivalDate =>
        x => x.ArrivalTime.Date == _endDate.Value.Date;
    
    public Expression<Func<Flight, bool>> BelongsToDepartureDate =>
        x => x.DepartureTime.Date == _startDate.Value.Date;

    public Expression<Func<Flight, bool>> FallsInDateRange =>
        x => (x.DepartureTime.Date >= _startDate.Value.Date && x.ArrivalTime.Date <= _endDate.Value.Date);
}

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplyAllFilters<T>(this IQueryable<T> queryable, IEnumerable<Expression<Func<T, bool>>> filters)
    {
        foreach (var filter in filters)
        {
            queryable = queryable.Where(filter);
        }

        return queryable;
    }
}