using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("V_AG_LINEE_POSTAZIONI")]
    public class VAgLineePostazioni
    {
        [Column("COD_LINEA_PROD")]
        public string CodLineaProd { get; set; } = string.Empty;

        [Column("COD_POSTAZIONE")]
        public string CodPostazione { get; set; } = string.Empty;
    }
}
