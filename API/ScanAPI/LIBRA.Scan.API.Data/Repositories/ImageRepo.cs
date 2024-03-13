using LIBRA.Scan.API.Data.EFs;
using LIBRA.Scan.API.Data.Repositories.Constracts;
using LIBRA.Scan.API.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Data.Repositories
{
    public class ImageRepo : GenericRepository<Image>, IImageRepo
    {
        public ImageRepo(ScanAppContext context) : base(context)
        {
        }
    }
}
