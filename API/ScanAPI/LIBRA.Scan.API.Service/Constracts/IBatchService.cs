using LIBRA.Scan.API.Entities.Dtos;
using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.ViewModels;
using System.Linq.Expressions;

namespace LIBRA.Scan.API.Service.Constracts
{
    public interface IBatchService
    {
        Task<long> Create(BatchCreateRequest request);
        Task<IEnumerable<Batch>> Get(Expression<Func<Batch, bool>> predicate);
        Task<IEnumerable<Batch>> GetAll();
        Task<BatchVM> GetByName(string batchName);
    }
}
