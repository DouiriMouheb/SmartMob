namespace api.DTOs
{
    public class AcquisizioneDto
    {
        public int ID { get; set; }
        public string? COD_LINEA { get; set; }
        public string? COD_POSTAZIONE { get; set; }
        public string? FOTO_SUPERIORE { get; set; }
        public string? FOTO_FRONTALE { get; set; }
        public bool? ESITO_CQ_ARTICOLO { get; set; }
        public decimal? SCOSTAMENTO_CQ_ARTICOLO { get; set; }
        public string? CODICE_ARTICOLO { get; set; }
        public string? CODICE_ORDINE { get; set; }
        public DateTime? DT_INS { get; set; }
        public DateTime? DT_AGG { get; set; }
        public bool? ABILITA_CQ { get; set; }
    }

    public class AcquisizioneRealtimeUpdateDto
    {
        public string ChangeType { get; set; } = string.Empty; // "INSERT", "UPDATE", "DELETE"
        public AcquisizioneDto? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
