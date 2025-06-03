using FlightInformation.API.Entities;
using FlightInformation.API.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FlightInformation.API.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;
    private readonly ILogger<FlightsController> _logger;

    public FlightsController(IFlightService flightService, ILogger<FlightsController> logger)
    {
        _flightService = flightService;
        _logger = logger;
        
        _logger.LogInformation($"{nameof(FlightsController)} Started");
    }
    
    /// <summary>
    /// Get all flights.
    /// </summary>
    /// <returns>List of flights.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Flight>>> GetFlights()
    {
        try
        {
            _logger.LogInformation($"Executing {nameof(FlightsController)}.{nameof(GetFlights)} Method");
            var flights = await _flightService.GetFlights();
            return Ok(flights);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Get the flight by id.
    /// </summary>
    /// <param name="id">Flight Id.</param>
    /// <returns>Flight details if found, otherwise returns NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Flight>> GetFlight(int id)
    {
        try
        {
            _logger.LogInformation($"Executing {nameof(FlightsController)}.{nameof(GetFlight)} Method");
            Flight? flight = await _flightService.GetFlight(id);

            if (flight == null)
            {
                _logger.LogInformation($"{nameof(FlightsController)}.{nameof(GetFlight)}: {nameof(flight)} with {id} Not Found.");
                return NotFound($"{nameof(Flight)} with id: '{id}' not found.");
            }
            return Ok(await _flightService.GetFlight(id));
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Create a new flight.
    /// </summary>
    /// <param name="flight">Flight details for creation.</param>
    /// <returns>Created flight record if successful, otherwise BadRequest if data validation failed.</returns>
    [HttpPost]
    public async Task<ActionResult<Flight>> CreateFlight(Flight flight)
    {
        try
        {
            _logger.LogInformation($"Executing {nameof(FlightsController)}.{nameof(CreateFlight)} Method");
            var validateFlight = Helpers.Helper.FlightValidationError(flight);

            if (!string.IsNullOrEmpty(validateFlight))
            {
                _logger.LogInformation($"{nameof(FlightsController)}.{nameof(CreateFlight)}: {nameof(flight)} has data issue.");
                return BadRequest(validateFlight);
            }
        
            var createdFlight = await _flightService.AddFlight(flight);
            _logger.LogInformation($"{nameof(FlightsController)}.{nameof(CreateFlight)}: Successfully created {nameof(flight)} with Id: {createdFlight.Id}.");
        
            return CreatedAtAction(nameof(GetFlight), new { id = createdFlight.Id }, createdFlight);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Update the details of an existing flight record.
    /// </summary>
    /// <param name="id">Flight Id.</param>
    /// <param name="flight">Updated flight details.</param>
    /// <returns>The updated details of the flight if successful,
    /// otherwise NotFound if the Id mismatched or doesn't exist, or
    /// BadRequest if there is an issue with the request or record.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<Flight>> UpdateFlight(int id, Flight flight)
    {
        try
        {
            _logger.LogInformation($"Executing {nameof(FlightsController)}.{nameof(UpdateFlight)} Method");
            if (id != flight.Id)
            {
                _logger.LogInformation($"{nameof(FlightsController)}.{nameof(UpdateFlight)}: Provided Id {id} mismatched with {flight.Id}.");
                return NotFound($"Flight '{nameof(id)}' mismatched");
            }

            var validateFlight = Helpers.Helper.FlightValidationError(flight);
        
            if (!string.IsNullOrEmpty(validateFlight))
            {
                _logger.LogInformation($"{nameof(FlightsController)}.{nameof(UpdateFlight)}: {nameof(flight)} has data issue.");
                return BadRequest(validateFlight);
            }
        
            var updateResult = await _flightService.UpdateFlight(flight);

            if (updateResult == null)
            {
                _logger.LogError($"{nameof(FlightsController)}.{nameof(UpdateFlight)}: Error processing {nameof(flight)} with Id: {id}.");
                return BadRequest("Error updating flight.");
            }
        
            return Ok(updateResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Delete the flight by Id.
    /// </summary>
    /// <param name="id">Flight Id.</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFlight(int id)
    {
        try
        {
            _logger.LogInformation($"Executing {nameof(FlightsController)}.{nameof(DeleteFlight)} Method");
            var flightToDelete = await _flightService.GetFlight(id);

            if (flightToDelete == null)
            {
                _logger.LogInformation($"{nameof(FlightsController)}.{nameof(DeleteFlight)}: {nameof(Flight)} with {id} Not Found.");
                return NotFound("Flight not found");
            }
            
            await _flightService.DeleteFlight(flightToDelete);
            _logger.LogInformation($"{nameof(FlightsController)}.{nameof(DeleteFlight)}: {nameof(Flight)} with Id {id} has been deleted.");
            
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Search the records by using the criteria provided.
    /// </summary>
    /// <param name="airline">Name of the airline.</param>
    /// <param name="airport">Airport Code.</param>
    /// <param name="startDate">Date for range filter, if no endDate provided,
    /// this is used to filter records for the departure date.</param>
    /// <param name="endDate">Date for range filter, if no startDate provided,
    /// this is used to filter records for the arrival date.</param>
    /// <returns>List of flights if criteria matched, otherwise NotFound if no criteria provided.</returns>
    [HttpGet("search/")]
    public async Task<ActionResult<Flight>> SearchFlights([FromQuery] string? airline, [FromQuery] string? airport,
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            _logger.LogInformation($"Executing {nameof(FlightsController)}.{nameof(SearchFlights)} Method");

            // Check whether there are criteria provided
            if (!Request.Query.Any())
            {
                _logger.LogInformation(
                    $"{nameof(FlightsController)}.{nameof(SearchFlights)}: Query parameters were not provided.");
                return NotFound("Flight not found");
            }

            return Ok(await _flightService.SearchFlights(airline, airport, startDate, endDate));
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(500, e.Message);
        }
    }
}