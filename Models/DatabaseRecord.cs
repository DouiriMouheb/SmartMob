using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("TCFG_PARAMETRI_GENERALI")]
    public class DatabaseRecord
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string DESCRIZIONE { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string VALORE { get; set; } = string.Empty;
        
        [Column("DT_AGG")]
        public DateTime DT_AGG { get; set; }
        
        [Column("COD_LINEA_PROD")]
        [MaxLength(10)]
        public string? COD_LINEA_PROD { get; set; }
        
        [Column("COD_POSTAZIONE")]
        [MaxLength(10)]
        public string? COD_POSTAZIONE { get; set; }
        
        [Column("TIPOLOGIA")]
        public int? TIPOLOGIA { get; set; }
    }
}