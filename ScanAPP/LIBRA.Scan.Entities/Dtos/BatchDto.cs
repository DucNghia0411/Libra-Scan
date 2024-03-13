
namespace LIBRA.Scan.Entities.Dtos
{
    public class BatchDto
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public long StatusId { get; set; }

        public long JobId { get; set; }

        public bool? Deleted { get; set; }

        public virtual JobDto Job { get; set; } = null!;

        public virtual StatusDto Status { get; set; } = null!;
    }
}
