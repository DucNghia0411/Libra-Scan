using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Entities.ViewModels
{
    public class BatchVM
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public long UserId { get; set; }

        public bool Deleted { get; set; }

        public string? Note { get; set; }

        public string Path { get; set; } = null!;

        public DateTime CreatedDate { get; set; }
    }
}
