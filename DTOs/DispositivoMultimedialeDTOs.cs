using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class CreateDispositivoMultimedialeDto
    {
        [Required]
        [MaxLength(50)]
        public string CodLineaProd { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string CodPostazione { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string SerialeDispositivo { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string PathStorageDispositivo { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? PathDestinazioneSpostamento { get; set; }
    }

    public class UpdateDispositivoMultimedialeDto
    {
        [Required]
        [MaxLength(50)]
        public string CodLineaProd { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string CodPostazione { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string SerialeDispositivo { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string PathStorageDispositivo { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? PathDestinazioneSpostamento { get; set; }
    }

    public class DispositivoMultimedialeResponseDto
    {
        public int Id { get; set; }
        public string CodLineaProd { get; set; } = string.Empty;
        public string CodPostazione { get; set; } = string.Empty;
        public string SerialeDispositivo { get; set; } = string.Empty;
        public string PathStorageDispositivo { get; set; } = string.Empty;
        public string? PathDestinazioneSpostamento { get; set; }
        public DateTime DtIns { get; set; }
        public DateTime DtAgg { get; set; }
        public string FormattedDtIns { get; set; } = string.Empty;
        public string FormattedDtAgg { get; set; } = string.Empty;
    }
}
