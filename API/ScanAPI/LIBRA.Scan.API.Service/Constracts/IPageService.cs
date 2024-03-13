using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service.Constracts
{
    public interface IPageService
    {
        Task<long> Create(PageCreateRequest request);
        Task<IEnumerable<Page>> Get(Expression<Func<Page, bool>> predicate);
    }
}
