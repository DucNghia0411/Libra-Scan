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
    public interface IDocumentService
    {
        Task<long> Create(DocumentCreateRequest request);
        Task<Document> GetById(long documentId);
    }
}
