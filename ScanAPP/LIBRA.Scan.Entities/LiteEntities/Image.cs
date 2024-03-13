using System.Collections.Generic;

namespace LIBRA.Scan.Entities.LiteEntities
{
    public class Image
    {
        public long Id { get; set; } // integer
        public long? Page_Id { get; set; } // integer
        public string Name { get; set; } = null!; // text(max)
        public string? Path { get; set; } // text(max)
        public long? Number_Order { get; set; } // integer
        public long Deleted { get; set; } // integer
        public string? Note { get; set; } // text(max)
        public string Icode { get; set; } = null!; // text(max)
        public virtual ICollection<History> Histories { get; set; } = new List<History>();
        public virtual Page? Page { get; set; }
    }
}
