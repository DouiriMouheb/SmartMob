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
                        FormattedDate = r.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss")
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

     
    
        // PUT: api/DatabaseRecords/5
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

                record.DESCRIZIONE = updateDto.Descrizione;
                record.VALORE = updateDto.Valore;
                record.DT_AGG = DateTime.Now; // Update timestamp

                await _context.SaveChangesAsync();

                var responseDto = new RecordResponseDto
                {
                    Id = record.ID,
                    Description = record.DESCRIZIONE,
                    Value = record.VALORE,
                    DateAdded = record.DT_AGG,
                    FormattedDate = record.DT_AGG.ToString("yyyy-MM-dd HH:mm:ss")
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
}
}
