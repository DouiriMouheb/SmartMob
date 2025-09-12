using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.Models;
using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcquisizioniController : ControllerBase
    {
        private readonly IAcquisizioniRealtimeService _realtimeService;
        private readonly ApplicationDBContext _context;
        private readonly ILogger<AcquisizioniController> _logger;

        public AcquisizioniController(
            IAcquisizioniRealtimeService realtimeService,
            ApplicationDBContext context,
            ILogger<AcquisizioniController> logger)
        {
            _realtimeService = realtimeService;
            _context = context;
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
        /// Get all ACQUISIZIONI records (no filtering)
        /// </summary>
        /// <returns>All acquisizioni records</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Acquisizione>>> GetAll()
        {
            try
            {
                var acquisizioni = await _context.Acquisizioni
                    .OrderByDescending(a => a.ID)
                    .ToListAsync();
                return Ok(acquisizioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all acquisizioni");
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

        // FILTERED ENDPOINTS
        
        /// <summary>
        /// Get latest records for a specific production line
        /// </summary>
        /// <param name="codLinea">Production line code</param>
        /// <returns>Latest acquisizioni for the specified line</returns>
        [HttpGet("latest/line/{codLinea}")]
        public async Task<ActionResult<IEnumerable<Acquisizione>>> GetLatestByLine(string codLinea)
        {
            try
            {
                var acquisizioni = await _realtimeService.GetLatestByLineAsync(codLinea);
                return Ok(acquisizioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving latest acquisizioni for line {codLinea}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get latest records for a specific workstation
        /// </summary>
        /// <param name="codPostazione">Workstation code</param>
        /// <returns>Latest acquisizioni for the specified station</returns>
        [HttpGet("latest/station/{codPostazione}")]
        public async Task<ActionResult<IEnumerable<Acquisizione>>> GetLatestByStation(string codPostazione)
        {
            try
            {
                var acquisizioni = await _realtimeService.GetLatestByStationAsync(codPostazione);
                return Ok(acquisizioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving latest acquisizioni for station {codPostazione}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get latest records for a specific line and station combination
        /// </summary>
        /// <param name="codLinea">Production line code</param>
        /// <param name="codPostazione">Workstation code</param>
        /// <returns>Latest acquisizioni for the specified line and station</returns>
        [HttpGet("latest/line/{codLinea}/station/{codPostazione}")]
        public async Task<ActionResult<IEnumerable<Acquisizione>>> GetLatestByLineAndStation(string codLinea, string codPostazione)
        {
            try
            {
                var acquisizioni = await _realtimeService.GetLatestByLineAndStationAsync(codLinea, codPostazione);
                return Ok(acquisizioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving latest acquisizioni for line {codLinea} and station {codPostazione}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get single latest record for a specific production line
        /// </summary>
        /// <param name="codLinea">Production line code</param>
        /// <returns>Latest single acquisizione for the specified line</returns>
        [HttpGet("latest-single/line/{codLinea}")]
        public async Task<ActionResult<Acquisizione>> GetLatestSingleByLine(string codLinea)
        {
            try
            {
                var acquisition = await _realtimeService.GetLatestSingleByLineAsync(codLinea);
                if (acquisition == null)
                {
                    return NotFound($"No records found for line {codLinea}");
                }
                return Ok(acquisition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving latest single acquisizione for line {codLinea}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get single latest record for a specific workstation
        /// </summary>
        /// <param name="codPostazione">Workstation code</param>
        /// <returns>Latest single acquisizione for the specified station</returns>
        [HttpGet("latest-single/station/{codPostazione}")]
        public async Task<ActionResult<Acquisizione>> GetLatestSingleByStation(string codPostazione)
        {
            try
            {
                var acquisition = await _realtimeService.GetLatestSingleByStationAsync(codPostazione);
                if (acquisition == null)
                {
                    return NotFound($"No records found for station {codPostazione}");
                }
                return Ok(acquisition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving latest single acquisizione for station {codPostazione}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get single latest record for a specific line and station combination
        /// </summary>
        /// <param name="codLinea">Production line code</param>
        /// <param name="codPostazione">Workstation code</param>
        /// <returns>Latest single acquisizione for the specified line and station</returns>
        [HttpGet("latest-single/line/{codLinea}/station/{codPostazione}")]
        public async Task<ActionResult<Acquisizione>> GetLatestSingleByLineAndStation(string codLinea, string codPostazione)
        {
            try
            {
                var acquisition = await _realtimeService.GetLatestSingleByLineAndStationAsync(codLinea, codPostazione);
                if (acquisition == null)
                {
                    return NotFound($"No records found for line {codLinea} and station {codPostazione}");
                }
                return Ok(acquisition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving latest single acquisizione for line {codLinea} and station {codPostazione}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
