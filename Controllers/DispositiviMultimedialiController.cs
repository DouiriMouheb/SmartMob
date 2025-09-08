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
    [Route("api/DispositiviMultimediali")]
    [ApiController]
    public class DispositiviMultimedialiController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        
        public DispositiviMultimedialiController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/DispositiviMultimediali
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DispositivoMultimedialeResponseDto>>> GetAll()
        {
            try
            {
                var dispositivi = await _context.DispositiviMultimediali
                    .OrderBy(d => d.Id)
                    .Select(d => new DispositivoMultimedialeResponseDto
                    {
                        Id = d.Id,
                        CodLineaProd = d.CodLineaProd,
                        CodPostazione = d.CodPostazione,
                        SerialeDispositivo = d.SerialeDispositivo,
                        PathStorageDispositivo = d.PathStorageDispositivo,
                        PathDestinazioneSpostamento = d.PathDestinazioneSpostamento,
                        DtIns = d.DtIns,
                        DtAgg = d.DtAgg,
                        FormattedDtIns = d.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormattedDtAgg = d.DtAgg.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToListAsync();

                return Ok(dispositivi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving dispositivi multimediali: {ex.Message}");
            }
        }

        // GET: api/DispositiviMultimediali/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DispositivoMultimedialeResponseDto>> GetById(int id)
        {
            try
            {
                var dispositivo = await _context.DispositiviMultimediali
                    .Where(d => d.Id == id)
                    .Select(d => new DispositivoMultimedialeResponseDto
                    {
                        Id = d.Id,
                        CodLineaProd = d.CodLineaProd,
                        CodPostazione = d.CodPostazione,
                        SerialeDispositivo = d.SerialeDispositivo,
                        PathStorageDispositivo = d.PathStorageDispositivo,
                        PathDestinazioneSpostamento = d.PathDestinazioneSpostamento,
                        DtIns = d.DtIns,
                        DtAgg = d.DtAgg,
                        FormattedDtIns = d.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormattedDtAgg = d.DtAgg.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .FirstOrDefaultAsync();

                if (dispositivo == null)
                {
                    return NotFound($"Dispositivo multimediale with ID {id} not found.");
                }

                return Ok(dispositivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving dispositivo multimediale: {ex.Message}");
            }
        }

        // POST: api/DispositiviMultimediali
        [HttpPost]
        public async Task<ActionResult<DispositivoMultimedialeResponseDto>> Create([FromBody] CreateDispositivoMultimedialeDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if a device with the same combination already exists
                var existingDevice = await _context.DispositiviMultimediali
                    .FirstOrDefaultAsync(d => d.CodLineaProd == createDto.CodLineaProd && 
                                            d.CodPostazione == createDto.CodPostazione && 
                                            d.SerialeDispositivo == createDto.SerialeDispositivo);

                if (existingDevice != null)
                {
                    return Conflict($"A device with the combination of production line '{createDto.CodLineaProd}', workstation '{createDto.CodPostazione}', and serial number '{createDto.SerialeDispositivo}' already exists.");
                }

                var now = DateTime.Now;
                var dispositivo = new DispositivoMultimediale
                {
                    CodLineaProd = createDto.CodLineaProd,
                    CodPostazione = createDto.CodPostazione,
                    SerialeDispositivo = createDto.SerialeDispositivo,
                    PathStorageDispositivo = createDto.PathStorageDispositivo,
                    PathDestinazioneSpostamento = createDto.PathDestinazioneSpostamento,
                    DtIns = now,
                    DtAgg = now
                };

                _context.DispositiviMultimediali.Add(dispositivo);
                await _context.SaveChangesAsync();

                var responseDto = new DispositivoMultimedialeResponseDto
                {
                    Id = dispositivo.Id,
                    CodLineaProd = dispositivo.CodLineaProd,
                    CodPostazione = dispositivo.CodPostazione,
                    SerialeDispositivo = dispositivo.SerialeDispositivo,
                    PathStorageDispositivo = dispositivo.PathStorageDispositivo,
                    PathDestinazioneSpostamento = dispositivo.PathDestinazioneSpostamento,
                    DtIns = dispositivo.DtIns,
                    DtAgg = dispositivo.DtAgg,
                    FormattedDtIns = dispositivo.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                    FormattedDtAgg = dispositivo.DtAgg.ToString("yyyy-MM-dd HH:mm:ss")
                };

                return CreatedAtAction(nameof(GetById), new { id = dispositivo.Id }, responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating dispositivo multimediale: {ex.Message}");
            }
        }

        // PUT: api/DispositiviMultimediali/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<DispositivoMultimedialeResponseDto>> Update(int id, [FromBody] UpdateDispositivoMultimedialeDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var dispositivo = await _context.DispositiviMultimediali.FindAsync(id);
                if (dispositivo == null)
                {
                    return NotFound($"Dispositivo multimediale with ID {id} not found.");
                }

                // Check if another device with the same combination exists (excluding current device)
                var existingDevice = await _context.DispositiviMultimediali
                    .FirstOrDefaultAsync(d => d.CodLineaProd == updateDto.CodLineaProd && 
                                            d.CodPostazione == updateDto.CodPostazione && 
                                            d.SerialeDispositivo == updateDto.SerialeDispositivo && 
                                            d.Id != id);

                if (existingDevice != null)
                {
                    return Conflict($"Un altro dispositivo con la combinazione della linea di produzione '{updateDto.CodLineaProd}', Cod. Postazione '{updateDto.CodPostazione}', e Seriale Dispositivo '{updateDto.SerialeDispositivo}' esiste già.");
                }

                // Update properties
                dispositivo.CodLineaProd = updateDto.CodLineaProd;
                dispositivo.CodPostazione = updateDto.CodPostazione;
                dispositivo.SerialeDispositivo = updateDto.SerialeDispositivo;
                dispositivo.PathStorageDispositivo = updateDto.PathStorageDispositivo;
                dispositivo.PathDestinazioneSpostamento = updateDto.PathDestinazioneSpostamento;
                dispositivo.DtAgg = DateTime.Now;

                await _context.SaveChangesAsync();

                var responseDto = new DispositivoMultimedialeResponseDto
                {
                    Id = dispositivo.Id,
                    CodLineaProd = dispositivo.CodLineaProd,
                    CodPostazione = dispositivo.CodPostazione,
                    SerialeDispositivo = dispositivo.SerialeDispositivo,
                    PathStorageDispositivo = dispositivo.PathStorageDispositivo,
                    PathDestinazioneSpostamento = dispositivo.PathDestinazioneSpostamento,
                    DtIns = dispositivo.DtIns,
                    DtAgg = dispositivo.DtAgg,
                    FormattedDtIns = dispositivo.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                    FormattedDtAgg = dispositivo.DtAgg.ToString("yyyy-MM-dd HH:mm:ss")
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating dispositivo multimediale: {ex.Message}");
            }
        }

        // DELETE: api/DispositiviMultimediali/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dispositivo = await _context.DispositiviMultimediali.FindAsync(id);
                if (dispositivo == null)
                {
                    return NotFound($"Dispositivo multimediale with ID {id} not found.");
                }

                _context.DispositiviMultimediali.Remove(dispositivo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting dispositivo multimediale: {ex.Message}");
            }
        }

        // GET: api/DispositiviMultimediali/bySerial/{serialeDispositivo}
        [HttpGet("bySerial/{serialeDispositivo}")]
        public async Task<ActionResult<DispositivoMultimedialeResponseDto>> GetBySerial(string serialeDispositivo)
        {
            try
            {
                var dispositivo = await _context.DispositiviMultimediali
                    .Where(d => d.SerialeDispositivo == serialeDispositivo)
                    .Select(d => new DispositivoMultimedialeResponseDto
                    {
                        Id = d.Id,
                        CodLineaProd = d.CodLineaProd,
                        CodPostazione = d.CodPostazione,
                        SerialeDispositivo = d.SerialeDispositivo,
                        PathStorageDispositivo = d.PathStorageDispositivo,
                        PathDestinazioneSpostamento = d.PathDestinazioneSpostamento,
                        DtIns = d.DtIns,
                        DtAgg = d.DtAgg,
                        FormattedDtIns = d.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormattedDtAgg = d.DtAgg.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .FirstOrDefaultAsync();

                if (dispositivo == null)
                {
                    return NotFound($"Dispositivo multimediale with serial '{serialeDispositivo}' not found.");
                }

                return Ok(dispositivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving dispositivo multimediale: {ex.Message}");
            }
        }

        // GET: api/DispositiviMultimediali/byLinea/{codLineaProd}
        [HttpGet("byLinea/{codLineaProd}")]
        public async Task<ActionResult<IEnumerable<DispositivoMultimedialeResponseDto>>> GetByLineaProd(string codLineaProd)
        {
            try
            {
                var dispositivi = await _context.DispositiviMultimediali
                    .Where(d => d.CodLineaProd == codLineaProd)
                    .OrderBy(d => d.CodPostazione)
                    .Select(d => new DispositivoMultimedialeResponseDto
                    {
                        Id = d.Id,
                        CodLineaProd = d.CodLineaProd,
                        CodPostazione = d.CodPostazione,
                        SerialeDispositivo = d.SerialeDispositivo,
                        PathStorageDispositivo = d.PathStorageDispositivo,
                        PathDestinazioneSpostamento = d.PathDestinazioneSpostamento,
                        DtIns = d.DtIns,
                        DtAgg = d.DtAgg,
                        FormattedDtIns = d.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormattedDtAgg = d.DtAgg.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToListAsync();

                return Ok(dispositivi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving dispositivi multimediali: {ex.Message}");
            }
        }

        // GET: api/DispositiviMultimediali/byLineaPostazione/{codLineaProd}/{codPostazione}
        [HttpGet("byLineaPostazione/{codLineaProd}/{codPostazione}")]
        public async Task<ActionResult<IEnumerable<DispositivoMultimedialeResponseDto>>> GetByLineaAndPostazione(string codLineaProd, string codPostazione)
        {
            try
            {
                var dispositivi = await _context.DispositiviMultimediali
                    .Where(d => d.CodLineaProd == codLineaProd && d.CodPostazione == codPostazione)
                    .OrderBy(d => d.SerialeDispositivo)
                    .Select(d => new DispositivoMultimedialeResponseDto
                    {
                        Id = d.Id,
                        CodLineaProd = d.CodLineaProd,
                        CodPostazione = d.CodPostazione,
                        SerialeDispositivo = d.SerialeDispositivo,
                        PathStorageDispositivo = d.PathStorageDispositivo,
                        PathDestinazioneSpostamento = d.PathDestinazioneSpostamento,
                        DtIns = d.DtIns,
                        DtAgg = d.DtAgg,
                        FormattedDtIns = d.DtIns.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormattedDtAgg = d.DtAgg.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToListAsync();

                return Ok(dispositivi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving dispositivi multimediali: {ex.Message}");
            }
        }
    }
}
