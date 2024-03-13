namespace LIBRA.Scan.Entities.LiteEntities
{
    public class StatusScan
    {
        public long Id { get; set; } // integer
        public string Status { get; set; } = null!; // text(max)
        public string? Description { get; set; } // text(max)
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
