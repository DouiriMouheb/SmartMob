using api.Hubs;
using api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public interface IAcquisizioniRealtimeService
    {
        Task StartMonitoringAsync();
        Task StopMonitoringAsync();
        Task<IEnumerable<Acquisizione>> GetLatestAcquisizioniAsync();
        Task<Acquisizione?> GetLatestSingleRecordAsync();
    }

    public class AcquisizioniRealtimeService : BackgroundService, IAcquisizioniRealtimeService
    {
        private readonly IHubContext<AcquisizioniHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AcquisizioniRealtimeService> _logger;
        private readonly string _connectionString;
        private bool _isMonitoring = false;
        private int _lastKnownId = 0;
        private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(3);

        public AcquisizioniRealtimeService(
            IHubContext<AcquisizioniHub> hubContext,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<AcquisizioniRealtimeService> logger)
        {
            _hubContext = hubContext;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartMonitoringAsync();
            
            // Initialize the last known ID
            await InitializeLastKnownIdAsync();
            
            _logger.LogInformation("🚀 Starting polling-based ACQUISIZIONI monitoring every 3 seconds...");
            
            while (!stoppingToken.IsCancellationRequested && _isMonitoring)
            {
                try
                {
                    await CheckForNewRecordsAsync();
                    await Task.Delay(_pollingInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during polling check");
                    await Task.Delay(_pollingInterval, stoppingToken);
                }
            }
            
            await StopMonitoringAsync();
        }

        public Task StartMonitoringAsync()
        {
            _isMonitoring = true;
            _logger.LogInformation("✅ Started monitoring ACQUISIZIONI table with 3-second polling.");
            return Task.CompletedTask;
        }

        public Task StopMonitoringAsync()
        {
            _isMonitoring = false;
            _logger.LogInformation("⏹️ Stopped monitoring ACQUISIZIONI table.");
            return Task.CompletedTask;
        }

        private async Task InitializeLastKnownIdAsync()
        {
            try
            {
                var latestRecord = await GetLatestSingleRecordAsync();
                if (latestRecord != null)
                {
                    _lastKnownId = latestRecord.ID;
                    _logger.LogInformation($"📊 Initialized last known ID: {_lastKnownId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing last known ID");
            }
        }

        private async Task CheckForNewRecordsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                
                // Get records with ID greater than last known ID
                var newRecords = await context.Acquisizioni
                    .Where(a => a.ID > _lastKnownId)
                    .OrderBy(a => a.ID)
                    .ToListAsync();

                if (newRecords.Any())
                {
                    foreach (var newRecord in newRecords)
                    {
                        _logger.LogInformation($"🆕 New record detected: ID {newRecord.ID}, Line: {newRecord.COD_LINEA}");
                        
                        // Send the new record to all connected clients
                        await _hubContext.Clients.All.SendAsync("NewAcquisizioneAdded", newRecord);
                        
                        // Update last known ID
                        _lastKnownId = newRecord.ID;
                    }
                    
                    // Also send the latest record in the old format for backward compatibility
                    var latestRecord = newRecords.Last();
                    await _hubContext.Clients.All.SendAsync("AcquisizioniUpdated", new[] { latestRecord });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for new records");
            }
        }

        public async Task<IEnumerable<Acquisizione>> GetLatestAcquisizioniAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                
                // Get the latest single record ordered by ID descending (most recent)
                var latestRecord = await context.Acquisizioni
                    .OrderByDescending(a => a.ID)
                    .Take(1)
                    .ToListAsync();

                return latestRecord;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest acquisizioni");
                return Enumerable.Empty<Acquisizione>();
            }
        }

        public async Task<Acquisizione?> GetLatestSingleRecordAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                
                // Get the single latest record by ID
                var latestRecord = await context.Acquisizioni
                    .OrderByDescending(a => a.ID)
                    .FirstOrDefaultAsync();

                return latestRecord;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest single acquisizione record");
                return null;
            }
        }
    }
}
