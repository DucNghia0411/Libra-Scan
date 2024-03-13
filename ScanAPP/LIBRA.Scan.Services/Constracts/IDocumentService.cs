using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IDocumentService
    {
        Task<long> Create(DocumentCreateRequest request);
        IEnumerable<Document> GetDocuments();
        IEnumerable<Document> GetDocumentsByBatchId(long batchId);
        long Update(DocumentUpdateRequest request);
        bool Delete(long id);
    }
}
