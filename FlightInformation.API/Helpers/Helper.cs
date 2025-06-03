using System.ComponentModel.DataAnnotations;
using FlightInformation.API.Entities;

namespace FlightInformation.API.Helpers;

public static class Helper
{
    /// <summary>
    /// Checks if the flight object has valid data.
    /// </summary>
    /// <param name="flight">Flight details.</param>
    /// <returns>Returns null if the flight object does not have any data issue, otherwise the error message.</returns>
    public static string FlightValidationError(Flight flight)
    {
        if (flight == null)
        {
            return "No flight details provided.";
        }
        
        var type = flight.GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(flight);

            if (property.PropertyType == typeof(string) && string.IsNullOrEmpty(value as string))
            {
                var customAttributes = property.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
                return $"'{customAttributes[0].Name}' must contain a value.";
            }
        }
        
        return null;
    }
    
    public static string? ToLower(string? value) =>  value?.ToLower().Trim();
}