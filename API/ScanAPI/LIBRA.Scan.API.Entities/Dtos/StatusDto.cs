
namespace LIBRA.Scan.API.Entities.Dtos;

public partial class StatusDto
{
    public long Id { get; set; }

    public string? StatusName { get; set; }

    public string? StatusDesc { get; set; }

    public bool? Active { get; set; }
    public virtual ICollection<BatchDto>? Batches { get; set; } = new List<BatchDto>();
}
