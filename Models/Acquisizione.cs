using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("ACQUISIZIONI")]
    public class Acquisizione
    {
        [Key]
        public int ID { get; set; }
        
        public string? COD_LINEA { get; set; }
        public string? COD_POSTAZIONE { get; set; }
        public string? FOTO_SUPERIORE { get; set; }
        public string? FOTO_FRONTALE { get; set; }
        public string? FOTO_BOX { get; set; }
        public bool? ESITO_CQ_ARTICOLO { get; set; }
        public decimal? ESITO_CQ_BOX { get; set; }
        public decimal? CONFIDENZA_CQ_BOX { get; set; }
        public decimal? SCOSTAMENTO_CQ_ARTICOLO { get; set; }
        public string? CODICE_ARTICOLO { get; set; }
        public string? CODICE_ORDINE { get; set; }
        public DateTime? DT_INS { get; set; }
        public DateTime? DT_AGG { get; set; }
        public bool? ABILITA_CQ { get; set; }
    }
}
