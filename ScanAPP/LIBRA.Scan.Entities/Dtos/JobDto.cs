
namespace LIBRA.Scan.Entities.Dtos
{
    public class JobDto
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? Deleted { get; set; }

        public virtual ICollection<BatchDto> Batches { get; set; } = new List<BatchDto>();
    }
}
