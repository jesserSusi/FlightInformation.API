using System.Text.Json.Serialization;
using FlightInformation.API.Controllers;
using FlightInformation.API.Data;
using FlightInformation.API.Interface;
using FlightInformation.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{    
    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "Flight Information API",
            Version = "v1",
            Description = "A project with RESTful API for managing flight information using C# and .NET Core 8",
            Contact = new OpenApiContact() { Name = "Jesser Susi", Email = "susi.jesser@gmail.com" }
        });
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddDbContext<FlightDbContext>(options =>
        options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DbName")));
    builder.Services.AddScoped<IFlightService, FlightService>();
    builder.Services.AddSingleton<ILogger<FlightsController>, Logger<FlightsController>>();
    
    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(swg => 
            swg.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
    }

    app.UseHttpsRedirection();
    
    app.UseAuthorization();
    
    app.MapControllers();
    
    app.Run();
}