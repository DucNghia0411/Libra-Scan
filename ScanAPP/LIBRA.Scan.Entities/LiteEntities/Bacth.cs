using System.Collections.Generic;


namespace LIBRA.Scan.Entities.LiteEntities
{
    public class Batch
    {
        public long Id { get; set; } 
        public string Name { get; set; } = null!; 
        public string Created_Date { get; set; } = null!; 
        public long User_Id { get; set; } 
        public long Deleted { get; set; } 
        public string? Note { get; set; } 
        public string Path { get; set; } = null!; 
        public long Status { get; set; } 
        public string Icode { get; set; } = null!; 
        public virtual StatusBatch StatusBatch { get; set; } = null!;
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
