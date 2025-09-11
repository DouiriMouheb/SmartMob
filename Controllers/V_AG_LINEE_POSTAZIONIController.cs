using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.DTOs;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class V_AG_LINEE_POSTAZIONIController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<V_AG_LINEE_POSTAZIONIController> _logger;

        public V_AG_LINEE_POSTAZIONIController(ApplicationDBContext context, ILogger<V_AG_LINEE_POSTAZIONIController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/V_AG_LINEE_POSTAZIONI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineePostazioniDTO>>> GetLineePostazioni()
        {
            try
            {
                // First, get all data from the view
                var rawData = await _context.VAgLineePostazioni.ToListAsync();
                
                // Then group and transform in memory
                var data = rawData
                    .GroupBy(v => v.CodLineaProd)
                    .Select(g => new LineePostazioniDTO
                    {
                        COD_LINEA_PROD = g.Key,
                        COD_POSTAZIONE = g.Select(x => x.CodPostazione).Distinct().OrderBy(p => p).ToList()
                    })
                    .OrderBy(x => x.COD_LINEA_PROD)
                    .ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving V_AG_LINEE_POSTAZIONI data");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
