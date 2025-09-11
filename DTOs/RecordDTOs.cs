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

        [MaxLength(10)]
        public string? CodLineaProd { get; set; }

        [MaxLength(10)]
        public string? CodPostazione { get; set; }

        public int? Tipologia { get; set; }
    }

    public class UpdateRecordDto
    {
        [Required]
        [MaxLength(1000)]
        public string Valore { get; set; } = string.Empty;
    }

    public class UpdateFullRecordDto
    {
        [Required]
        [MaxLength(500)]
        public string Descrizione { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Valore { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? CodLineaProd { get; set; }

        [MaxLength(10)]
        public string? CodPostazione { get; set; }

        public int? Tipologia { get; set; }
    }

    public class RecordResponseDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
        public string FormattedDate { get; set; } = string.Empty;
        public string? CodLineaProd { get; set; }
        public string? CodPostazione { get; set; }
        public int? Tipologia { get; set; }
    }

    // DTOs for TipologiaSignificato (Read-only)
    public class TipologiaResponseDto
    {
        public int IdTipologia { get; set; }
        public string DesSignificato { get; set; } = string.Empty;
    }
}
