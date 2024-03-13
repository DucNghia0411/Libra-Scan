namespace LIBRA.Scan.Entities.LiteEntities
{
    public class StatusProcess
    {
        public long Id { get; set; } // integer
        public string Status { get; set; } = null!; // text(max)
        public long? Description { get; set; } // integer
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
