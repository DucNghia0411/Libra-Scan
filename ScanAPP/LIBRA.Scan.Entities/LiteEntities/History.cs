namespace LIBRA.Scan.Entities.LiteEntities
{
    public class History
    {
        public long Id { get; set; } // integer
        public long? DocumentId { get; set; } // integer
        public long? PageId { get; set; } // integer
        public string? Note { get; set; } // text(max)
        public string Actions { get; set; } = null!; // text(max)
        public string CreatedDate { get; set; } = null!; // text(max)
        public long UserId { get; set; } // integer
        public long? ImageId { get; set; } // integer
        public string Icode { get; set; } = null!; // text(max)
        public virtual Document? Document { get; set; }
        public virtual Image? Image { get; set; }
        public virtual Page? Page { get; set; }
    }
}
