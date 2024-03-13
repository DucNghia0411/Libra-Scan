using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IPageService
    {
        IEnumerable<Page> GetByDocumentId(long Id);
        Task<long> Create(PageCreateRequest request);
        Task<bool> CheckExists(string Icode);
        long GetCurrentOrderByDocumentId(long documentId);
    }
}
