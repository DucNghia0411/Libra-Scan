using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IBatchService
    {
        Task<long> Create(BatchCreateRequest request);
        IEnumerable<Batch> GetAll();
        IEnumerable<Batch> GetBatchesByUserId(long userId);
        bool Delete(long id);
        bool Update(BatchUpdateRequest request);
    }
}
