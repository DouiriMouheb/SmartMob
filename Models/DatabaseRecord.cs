using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("TCFG_PARAMETRI_GENERALI")] // You'll need to replace this with your actual table name
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
    }
}
