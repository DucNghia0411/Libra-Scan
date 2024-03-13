namespace LIBRA.Scan.Entities.LiteEntities;

public class StatusBatch
{
    public long Id { get; set; } // integer
    public string Status { get; set; } = null!; // text(max)
    public string? Description { get; set; } // text(max)
    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
