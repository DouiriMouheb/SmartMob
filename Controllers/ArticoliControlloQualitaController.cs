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

        // GET: api/ControlloQualita/5
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

        // GET: api/ControlloQualita/by-code/{codArticolo}
        [HttpGet("by-code/{codArticolo}")]
        public async Task<ActionResult<ArticoloResponseDto>> GetByCodArticolo(string codArticolo)
        {
            try
            {
                var articolo = await _context.ArticoliControlloQualita
                    .Where(a => a.CodArticolo == codArticolo)
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

                // Check if COD_ARTICOLO already exists
                var existingArticolo = await _context.ArticoliControlloQualita
                    .FirstOrDefaultAsync(a => a.CodArticolo == createDto.CodArticolo);
                
                if (existingArticolo != null)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = $"Articolo with code {createDto.CodArticolo} already exists" 
                    });
                }

                var articolo = new ArticoloControlloQualita
                {
                    CodArticolo = createDto.CodArticolo,
                    DtIns = DateTime.Now,
                    DtAgg = DateTime.Now,
                    CodLineaProd = createDto.CodLineaProd
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

        // PUT: api/ControlloQualita/5 (Update by ID)
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

                var articolo = await _context.ArticoliControlloQualita.FindAsync(id);
                if (articolo == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = $"Articolo with ID {id} not found" 
                    });
                }

                // Check if the new COD_ARTICOLO already exists (excluding current record)
                var existingArticolo = await _context.ArticoliControlloQualita
                    .FirstOrDefaultAsync(a => a.CodArticolo == updateDto.CodArticolo && a.Id != id);
                
                if (existingArticolo != null)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = $"Articolo with code {updateDto.CodArticolo} already exists" 
                    });
                }

                // Update fields
                articolo.CodArticolo = updateDto.CodArticolo;
                articolo.CodLineaProd = updateDto.CodLineaProd;
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

        // DELETE: api/ArticoliControlloQualita/5 (Delete by ID)
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var articolo = await _context.ArticoliControlloQualita.FindAsync(id);
                if (articolo == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = $"Articolo with ID {id} not found" 
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

        // GET: api/ControlloQualita/by-linea/{codLinea}
        [HttpGet("by-linea/{codLinea}")]
        public async Task<ActionResult<IEnumerable<ArticoloResponseDto>>> GetByLineaProd(string codLinea)
        {
            try
            {
                var articoli = await _context.ArticoliControlloQualita
                    .Where(a => a.CodLineaProd == codLinea)
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
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error retrieving articoli by linea", 
                    error = ex.Message 
                });
            }
        }
    }
}
