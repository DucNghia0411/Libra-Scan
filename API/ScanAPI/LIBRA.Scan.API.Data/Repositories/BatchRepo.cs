using LIBRA.Scan.API.Data.EFs;
using LIBRA.Scan.API.Data.Repositories.Constracts;
using LIBRA.Scan.API.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace LIBRA.Scan.API.Data.Repositories
{
    public class BatchRepo : GenericRepository<Batch>, IBatchRepo
    {
        public BatchRepo(ScanAppContext context) : base(context)
        {

        }
    }
}
