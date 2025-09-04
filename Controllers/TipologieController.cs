using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipologieController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public TipologieController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Tipologie
        [HttpGet]
        public async Task<ActionResult<object>> GetAllTipologie()
        {
            try
            {
                var tipologie = await _context.TipologieSignificati
                    .Select(t => new TipologiaResponseDto
                    {
                        IdTipologia = t.IdTipologia,
                        DesSignificato = t.DesSignificato
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    count = tipologie.Count,
                    data = tipologie
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving tipologie",
                    error = ex.Message
                });
            }
        }

        // GET: api/Tipologie/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTipologia(int id)
        {
            try
            {
                var tipologia = await _context.TipologieSignificati
                    .Where(t => t.IdTipologia == id)
                    .Select(t => new TipologiaResponseDto
                    {
                        IdTipologia = t.IdTipologia,
                        DesSignificato = t.DesSignificato
                    })
                    .FirstOrDefaultAsync();

                if (tipologia == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Tipologia not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = tipologia
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving tipologia",
                    error = ex.Message
                });
            }
        }
    }
}
