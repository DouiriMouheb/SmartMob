using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    [Table("Tcfg_DISPOSITIVI_MULTIMEDIALI")]
    public class DispositivoMultimediale
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("COD_LINEA_PROD")]
        [MaxLength(50)]
        [Required]
        public string CodLineaProd { get; set; } = string.Empty;
        
        [Column("COD_POSTAZIONE")]
        [MaxLength(50)]
        [Required]
        public string CodPostazione { get; set; } = string.Empty;
        
        [Column("SERIALE_DISPOSITIVO")]
        [MaxLength(100)]
        [Required]
        public string SerialeDispositivo { get; set; } = string.Empty;
        
        [Column("PATH_STORAGE_DISPOSITIVO")]
        [MaxLength(500)]
        [Required]
        public string PathStorageDispositivo { get; set; } = string.Empty;
        
        [Column("PATH_DESTINAZIONE_SPOSTAMENTO")]
        [MaxLength(500)]
        public string? PathDestinazioneSpostamento { get; set; }
        
        [Column("DT_INS")]
        public DateTime DtIns { get; set; }
        
        [Column("DT_AGG")]
        public DateTime DtAgg { get; set; }
    }
}
