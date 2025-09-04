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
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseRecordsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        
        public DatabaseRecordsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/DatabaseRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecordResponseDto>>> GetAll()
        {
            try
            {
                var records = await _context.DatabaseRecords
                    .OrderBy(r => r.ID)
                    .Select(r => new RecordResponseDto
                    {
                        Id = r.ID,
                        Description = r.DESCRIZIONE,
                        Value = r.VALORE,
                        DateAdded = r.DT_AGG,
                        FormattedDate = r.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss"),
                        CodLineaProd = r.COD_LINEA_PROD,
                        Tipologia = r.TIPOLOGIA
                    })
                    .ToListAsync();
                
                return Ok(new { 
                    success = true, 
                    count = records.Count,
                    data = records 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error retrieving data", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/DatabaseRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecordResponseDto>> GetById(int id)
        {
            try
            {
                var record = await _context.DatabaseRecords
                    .Where(r => r.ID == id)
                    .Select(r => new RecordResponseDto
                    {
                        Id = r.ID,
                        Description = r.DESCRIZIONE,
                        Value = r.VALORE,
                        DateAdded = r.DT_AGG,
                        FormattedDate = r.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss"),
                        CodLineaProd = r.COD_LINEA_PROD,
                        Tipologia = r.TIPOLOGIA
                    })
                    .FirstOrDefaultAsync();

                if (record == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = $"Record with ID {id} not found" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    data = record 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error retrieving record", 
                    error = ex.Message 
                });
            }
        }

        // POST: api/DatabaseRecords
        [HttpPost]
        public async Task<ActionResult<RecordResponseDto>> Create([FromBody] CreateRecordDto createDto)
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

                var record = new DatabaseRecord
                {
                    DESCRIZIONE = createDto.Descrizione,
                    VALORE = createDto.Valore,
                    DT_AGG = DateTime.Now,
                    COD_LINEA_PROD = createDto.CodLineaProd,
                    TIPOLOGIA = createDto.Tipologia
                };

                _context.DatabaseRecords.Add(record);
                await _context.SaveChangesAsync();

                var responseDto = new RecordResponseDto
                {
                    Id = record.ID,
                    Description = record.DESCRIZIONE,
                    Value = record.VALORE,
                    DateAdded = record.DT_AGG,
                    FormattedDate = record.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss"),
                    CodLineaProd = record.COD_LINEA_PROD,
                    Tipologia = record.TIPOLOGIA
                };

                return CreatedAtAction(nameof(GetById), new { id = record.ID }, new { 
                    success = true, 
                    message = "Record created successfully",
                    data = responseDto 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error creating record", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/DatabaseRecords/5 (Update only VALORE)
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateRecordDto updateDto)
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

                var record = await _context.DatabaseRecords.FindAsync(id);
                if (record == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = $"Record with ID {id} not found" 
                    });
                }

                // Only update the VALORE field, keep DESCRIZIONE unchanged
                record.VALORE = updateDto.Valore;
                record.DT_AGG = DateTime.Now; // Update timestamp

                await _context.SaveChangesAsync();

                var responseDto = new RecordResponseDto
                {
                    Id = record.ID,
                    Description = record.DESCRIZIONE,
                    Value = record.VALORE,
                    DateAdded = record.DT_AGG,
                    FormattedDate = record.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss"),
                    CodLineaProd = record.COD_LINEA_PROD,
                    Tipologia = record.TIPOLOGIA
                };

                return Ok(new { 
                    success = true, 
                    message = "Record updated successfully",
                    data = responseDto 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error updating record", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/DatabaseRecords/5/full (Update all fields)
        [HttpPut("{id}/full")]
        public async Task<ActionResult> UpdateFull(int id, [FromBody] UpdateFullRecordDto updateDto)
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

                var record = await _context.DatabaseRecords.FindAsync(id);
                if (record == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = $"Record with ID {id} not found" 
                    });
                }

                // Update all fields
                record.DESCRIZIONE = updateDto.Descrizione;
                record.VALORE = updateDto.Valore;
                record.COD_LINEA_PROD = updateDto.CodLineaProd;
                record.TIPOLOGIA = updateDto.Tipologia;
                record.DT_AGG = DateTime.Now; // Update timestamp

                await _context.SaveChangesAsync();

                var responseDto = new RecordResponseDto
                {
                    Id = record.ID,
                    Description = record.DESCRIZIONE,
                    Value = record.VALORE,
                    DateAdded = record.DT_AGG,
                    FormattedDate = record.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss"),
                    CodLineaProd = record.COD_LINEA_PROD,
                    Tipologia = record.TIPOLOGIA
                };

                return Ok(new { 
                    success = true, 
                    message = "Record fully updated successfully",
                    data = responseDto 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error updating record", 
                    error = ex.Message 
                });
            }
        }

        // DELETE: api/DatabaseRecords/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var record = await _context.DatabaseRecords.FindAsync(id);
                if (record == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = $"Record with ID {id} not found" 
                    });
                }

                _context.DatabaseRecords.Remove(record);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = $"Record with ID {id} deleted successfully" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error deleting record", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/DatabaseRecords/search?term=searchtext
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<RecordResponseDto>>> Search([FromQuery] string? term)
        {
            try
            {
                var query = _context.DatabaseRecords.AsQueryable();

                if (!string.IsNullOrEmpty(term))
                {
                    query = query.Where(r => r.DESCRIZIONE.Contains(term) || r.VALORE.Contains(term));
                }

                var records = await query
                    .OrderBy(r => r.ID)
                    .Select(r => new RecordResponseDto
                    {
                        Id = r.ID,
                        Description = r.DESCRIZIONE,
                        Value = r.VALORE,
                        DateAdded = r.DT_AGG,
                        FormattedDate = r.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss"),
                        CodLineaProd = r.COD_LINEA_PROD,
                        Tipologia = r.TIPOLOGIA
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    count = records.Count,
                    searchTerm = term,
                    data = records 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error searching records", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/DatabaseRecords/by-tipologia/1
        [HttpGet("by-tipologia/{tipologia}")]
        public async Task<ActionResult<IEnumerable<RecordResponseDto>>> GetByTipologia(int tipologia)
        {
            try
            {
                var records = await _context.DatabaseRecords
                    .Where(r => r.TIPOLOGIA == tipologia)
                    .OrderBy(r => r.ID)
                    .Select(r => new RecordResponseDto
                    {
                        Id = r.ID,
                        Description = r.DESCRIZIONE,
                        Value = r.VALORE,
                        DateAdded = r.DT_AGG,
                        FormattedDate = r.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss"),
                        CodLineaProd = r.COD_LINEA_PROD,
                        Tipologia = r.TIPOLOGIA
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    count = records.Count,
                    tipologia = tipologia,
                    data = records 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error retrieving records by tipologia", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/DatabaseRecords/by-linea/{codLinea}
        [HttpGet("by-linea/{codLinea}")]
        public async Task<ActionResult<IEnumerable<RecordResponseDto>>> GetByLineaProd(string codLinea)
        {
            try
            {
                var records = await _context.DatabaseRecords
                    .Where(r => r.COD_LINEA_PROD == codLinea)
                    .OrderBy(r => r.ID)
                    .Select(r => new RecordResponseDto
                    {
                        Id = r.ID,
                        Description = r.DESCRIZIONE,
                        Value = r.VALORE,
                        DateAdded = r.DT_AGG,
                        FormattedDate = r.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss"),
                        CodLineaProd = r.COD_LINEA_PROD,
                        Tipologia = r.TIPOLOGIA
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    count = records.Count,
                    codLineaProd = codLinea,
                    data = records 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error retrieving records by linea produzione", 
                    error = ex.Message 
                });
            }
        }
    }
}
