using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcquisizioniFilterController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<AcquisizioniFilterController> _logger;

        public AcquisizioniFilterController(ApplicationDBContext context, ILogger<AcquisizioniFilterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get filtered ACQUISIZIONI records
        /// </summary>
        /// <param name="codLineaProd">Production line code (optional)</param>
        /// <param name="codPostazione">Workstation code (optional)</param>
        /// <param name="limit">Maximum number of records to return (optional, default: 100)</param>
        /// <returns>Filtered acquisizioni records</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Acquisizione>>> GetFiltered(
            string? codLineaProd = null, 
            string? codPostazione = null, 
            int limit = 100)
        {
            try
            {
                var query = _context.Acquisizioni.AsQueryable();

                if (!string.IsNullOrEmpty(codLineaProd))
                {
                    query = query.Where(a => a.COD_LINEA == codLineaProd);
                }

                if (!string.IsNullOrEmpty(codPostazione))
                {
                    query = query.Where(a => a.COD_POSTAZIONE == codPostazione);
                }

                var acquisizioni = await query
                    .OrderByDescending(a => a.ID)
                    .Take(limit)
                    .ToListAsync();

                return Ok(acquisizioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filtered acquisizioni");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
