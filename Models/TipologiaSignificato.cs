using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("tcfg_SIGNIFICATI_TIPOLOGIE_PARAMETRI")]
    public class TipologiaSignificato
    {
        [Key]
        [Column("ID_TIPOLOGIA")]
        public int IdTipologia { get; set; }
        
        [Required]
        [Column("DES_SIGNIFICATO")]
        [MaxLength(1000)]
        public string DesSignificato { get; set; } = string.Empty;
    }
}
