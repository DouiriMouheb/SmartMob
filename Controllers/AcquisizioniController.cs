using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcquisizioniController : ControllerBase
    {
        private readonly IAcquisizioniRealtimeService _realtimeService;
        private readonly ILogger<AcquisizioniController> _logger;

        public AcquisizioniController(
            IAcquisizioniRealtimeService realtimeService,
            ILogger<AcquisizioniController> logger)
        {
            _realtimeService = realtimeService;
            _logger = logger;
        }

        /// <summary>
        /// Get the latest ACQUISIZIONI records
        /// </summary>
        /// <returns>List of latest acquisizioni</returns>
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<Acquisizione>>> GetLatest()
        {
            try
            {
                var acquisizioni = await _realtimeService.GetLatestAcquisizioniAsync();
                return Ok(acquisizioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest acquisizioni");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get the single latest ACQUISIZIONI record (most recent)
        /// </summary>
        /// <returns>Latest single acquisizione record</returns>
        [HttpGet("latest-single")]
        public async Task<ActionResult<Acquisizione>> GetLatestSingle()
        {
            try
            {
                var latestRecord = await _realtimeService.GetLatestSingleRecordAsync();
                if (latestRecord == null)
                {
                    return NotFound("No records found");
                }
                return Ok(latestRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest single acquisizione");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Manually trigger monitoring start (for testing purposes)
        /// </summary>
        [HttpPost("start-monitoring")]
        public async Task<ActionResult> StartMonitoring()
        {
            try
            {
                await _realtimeService.StartMonitoringAsync();
                return Ok("Monitoring started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting monitoring");
                return StatusCode(500, "Failed to start monitoring");
            }
        }

        /// <summary>
        /// Manually trigger monitoring stop (for testing purposes)
        /// </summary>
        [HttpPost("stop-monitoring")]
        public async Task<ActionResult> StopMonitoring()
        {
            try
            {
                await _realtimeService.StopMonitoringAsync();
                return Ok("Monitoring stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping monitoring");
                return StatusCode(500, "Failed to stop monitoring");
            }
        }
    }
}
