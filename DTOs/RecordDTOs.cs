using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class CreateRecordDto
    {
        [Required]
        [MaxLength(500)]
        public string Descrizione { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Valore { get; set; } = string.Empty;
    }

    public class UpdateRecordDto
    {
        [Required]
        [MaxLength(500)]
        public string Descrizione { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Valore { get; set; } = string.Empty;
    }

    public class RecordResponseDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
        public string FormattedDate { get; set; } = string.Empty;
    }
}
