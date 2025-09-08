using api.Data;
using api.Hubs;
using api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers
builder.Services.AddControllers();

// Add SignalR
builder.Services.AddSignalR();

// Add our real-time service
builder.Services.AddSingleton<IAcquisizioniRealtimeService, AcquisizioniRealtimeService>();
builder.Services.AddHostedService<AcquisizioniRealtimeService>(provider => 
    (AcquisizioniRealtimeService)provider.GetRequiredService<IAcquisizioniRealtimeService>());

// Add CORS for React frontend and external access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // More restrictive policy for production (optional)
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
            {
                // Allow any localhost origin for development
                if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return uri.Host == "localhost" || 
                           uri.Host == "127.0.0.1" ||
                           uri.Host.StartsWith("192.168.") ||
                           uri.Host.StartsWith("10.") ||
                           uri.Host.StartsWith("172.");
                }
                return false;
            })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Important for SignalR
    });
});

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS - Allow specific origins with credentials for SignalR
app.UseCors(builder => builder
    .WithOrigins("http://localhost:5173", "http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Map SignalR Hub
app.MapHub<AcquisizioniHub>("/hubs/acquisizioni");

app.Run();

