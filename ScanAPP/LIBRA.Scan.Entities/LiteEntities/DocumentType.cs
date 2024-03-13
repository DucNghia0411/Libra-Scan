namespace LIBRA.Scan.Entities.LiteEntities
{
    public class DocumentType
    {
        public long Id { get; set; } // integer
        public string Type { get; set; } = null!; // text(max)
        public string? Note { get; set; } // text(max)
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
