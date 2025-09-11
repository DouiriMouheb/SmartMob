namespace api.DTOs
{
    public class LineePostazioniDTO
    {
        public string COD_LINEA_PROD { get; set; } = string.Empty;
        public List<string> COD_POSTAZIONE { get; set; } = new List<string>();
    }
}
