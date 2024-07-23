using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Certificate path and password
var certFileLocation = @"D:\Docker\SSL\weather.pfx";
var certPassword = "weather";

// Check if the certificate file exists
if (!File.Exists(certFileLocation))
{
    Console.WriteLine($"Certificate file not found at: {certFileLocation}");
    throw new FileNotFoundException("Certificate file not found.", certFileLocation);
}

// Configure Kestrel in the builder
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(certFileLocation, certPassword);
    });
    options.ListenAnyIP(5000); // HTTP port
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseAuthorization();
app.MapControllers();

app.Run();
