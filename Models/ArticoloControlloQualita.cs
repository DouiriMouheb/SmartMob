using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("ARTICOLI_CONTROLLO_QUALITA")]
    public class ArticoloControlloQualita
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        
        [Column("COD_ARTICOLO")]
        [MaxLength(50)]
        [Required]
        public string CodArticolo { get; set; } = string.Empty;
        
        [Column("DT_INS")]
        public DateTime DtIns { get; set; }
        
        [Column("DT_AGG")]
        public DateTime DtAgg { get; set; }
        
        [Column("COD_LINEA_PROD")]
        [MaxLength(10)]
        public string? CodLineaProd { get; set; }
    }
}
