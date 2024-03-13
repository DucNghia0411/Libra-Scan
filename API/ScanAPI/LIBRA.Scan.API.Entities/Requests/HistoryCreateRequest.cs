using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Entities.Requests
{
    public class HistoryCreateRequest
    {
        public long? DocumentId { get; set; }

        public int? PageId { get; set; }

        public string? Note { get; set; }

        public string Actions { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public long UserId { get; set; }

        public int? ImageId { get; set; }
    }
}
