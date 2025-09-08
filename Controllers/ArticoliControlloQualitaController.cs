using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/ControlloQualita")]
    [ApiController]
    public class ArticoliControlloQualitaController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        
        public ArticoliControlloQualitaController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/ControlloQualita
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticoloResponseDto>>> GetAll()
        {
            try
            {
                var articoli = await _context.ArticoliControlloQualita
                    .OrderBy(a => a.Id)
                    .Select(a => new ArticoloResponseDto
                    {
                        Id = a.Id,
                        CodArticolo = a.CodArticolo,
                        DtIns = a.DtIns,
                        DtAgg = a.DtAgg,
                        FormattedDtIns = a.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormattedDtAgg = a.DtAgg.ToString("yyyy-MM-dd HH:mm:ss"),
                        CodLineaProd = a.CodLineaProd
                    })
                    .ToListAsync();

                return Ok(articoli);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving articoli: {ex.Message}");
            }
        }

        // GET: api/ControlloQualita/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticoloResponseDto>> GetById(int id)
        {
            try
            {
                var articolo = await _context.ArticoliControlloQualita
                    .Where(a => a.Id == id)
                    .Select(a => new ArticoloResponseDto
                    {
                        Id = a.Id,
                        CodArticolo = a.CodArticolo,
                        DtIns = a.DtIns,
                        DtAgg = a.DtAgg,
                        FormattedDtIns = a.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormattedDtAgg = a.DtAgg.ToString("yyyy-MM-dd HH:mm:ss"),
                        CodLineaProd = a.CodLineaProd
                    })
                    .FirstOrDefaultAsync();

                if (articolo == null)
                {
                    return NotFound();
                }

                return Ok(articolo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error retrieving articolo", 
                    error = ex.Message 
                });
            }
        }

        // POST: api/ControlloQualita
        [HttpPost]
        public async Task<ActionResult<ArticoloResponseDto>> Create([FromBody] CreateArticoloDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Invalid data", 
                        errors = ModelState 
                    });
                }

                // Check if the combination of COD_ARTICOLO + COD_LINEA_PROD already exists
                var existingArticolo = await _context.ArticoliControlloQualita
                    .FirstOrDefaultAsync(a => a.CodArticolo == createDto.CodArticolo && a.CodLineaProd == createDto.CodLineaProd);
                
                if (existingArticolo != null)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = $"La combinazione di Articolo  '{createDto.CodArticolo}' e linea di produzione '{createDto.CodLineaProd}' already exists" 
                    });
                }

                var articolo = new ArticoloControlloQualita
                {
                    CodArticolo = createDto.CodArticolo,
                    DtIns = DateTime.Now,
                    DtAgg = DateTime.Now,
                    CodLineaProd = createDto.CodLineaProd ?? ""
                };

                _context.ArticoliControlloQualita.Add(articolo);
                await _context.SaveChangesAsync();

                var responseDto = new ArticoloResponseDto
                {
                    Id = articolo.Id,
                    CodArticolo = articolo.CodArticolo,
                    DtIns = articolo.DtIns,
                    DtAgg = articolo.DtAgg,
                    FormattedDtIns = articolo.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                    FormattedDtAgg = articolo.DtAgg.ToString("yyyy-MM-dd HH:mm:ss"),
                    CodLineaProd = articolo.CodLineaProd
                };

                return CreatedAtAction(nameof(GetById), new { id = articolo.Id }, responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error creating articolo", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/ControlloQualita/{id} (Update by ID)
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateArticoloDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Invalid data", 
                        errors = ModelState 
                    });
                }

                // Find the existing articolo by ID
                var articolo = await _context.ArticoliControlloQualita
                    .FirstOrDefaultAsync(a => a.Id == id);
                    
                if (articolo == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = $"Articolo with ID '{id}' not found" 
                    });
                }

                // Check if the new combination of COD_ARTICOLO + COD_LINEA_PROD already exists
                // (excluding the current record we're updating)
                var existingArticolo = await _context.ArticoliControlloQualita
                    .FirstOrDefaultAsync(a => a.CodArticolo == updateDto.CodArticolo && 
                                            a.CodLineaProd == updateDto.CodLineaProd &&
                                            a.Id != id); // Exclude current record by ID
                
                if (existingArticolo != null)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = $"La combinazione di Articolo  '{updateDto.CodArticolo}' e linea di produzione '{updateDto.CodLineaProd}' already exists" 
                    });
                }

                // Update fields
                articolo.CodArticolo = updateDto.CodArticolo;
                articolo.CodLineaProd = updateDto.CodLineaProd ?? "";
                articolo.DtAgg = DateTime.Now; // Update timestamp

                await _context.SaveChangesAsync();

                var responseDto = new ArticoloResponseDto
                {
                    Id = articolo.Id,
                    CodArticolo = articolo.CodArticolo,
                    DtIns = articolo.DtIns,
                    DtAgg = articolo.DtAgg,
                    FormattedDtIns = articolo.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                    FormattedDtAgg = articolo.DtAgg.ToString("yyyy-MM-dd HH:mm:ss"),
                    CodLineaProd = articolo.CodLineaProd
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error updating articolo", 
                    error = ex.Message 
                });
            }
        }

      // DELETE: api/ControlloQualita/{id} (Delete by ID)
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var articolo = await _context.ArticoliControlloQualita
                    .FirstOrDefaultAsync(a => a.Id == id);
                    
                if (articolo == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = $"Articolo with ID '{id}' not found" 
                    });
                }

                _context.ArticoliControlloQualita.Remove(articolo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error deleting articolo", 
                    error = ex.Message 
                });
            }
        }
    }
}
