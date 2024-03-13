using System.Collections.Generic;

namespace LIBRA.Scan.Entities.LiteEntities
{
    public class Page
    {
        public long Id { get; set; } // integer
        public string Name { get; set; } = null!; // text(max)
        public long Document_Id { get; set; } // integer
        public string? Path { get; set; } // text(max)
        public long? Number_Order { get; set; } // integer
        public long Deleted { get; set; } // integer
        public string Icode { get; set; } = null!; // text(max)
        public virtual Document Document { get; set; } = null!;

        public virtual ICollection<History> Histories { get; set; } = new List<History>();

        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
