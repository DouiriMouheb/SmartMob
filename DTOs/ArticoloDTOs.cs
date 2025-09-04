using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class CreateArticoloDto
    {
        [Required]
        [MaxLength(50)]
        public string CodArticolo { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? CodLineaProd { get; set; }
    }

    public class UpdateArticoloDto
    {
        [Required]
        [MaxLength(50)]
        public string CodArticolo { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? CodLineaProd { get; set; }
    }

    public class ArticoloResponseDto
    {
        public int Id { get; set; }
        public string CodArticolo { get; set; } = string.Empty;
        public DateTime DtIns { get; set; }
        public DateTime DtAgg { get; set; }
        public string FormattedDtIns { get; set; } = string.Empty;
        public string FormattedDtAgg { get; set; } = string.Empty;
        public string? CodLineaProd { get; set; }
    }
}
