using LIBRA.Scan.Entities.LiteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IDocumentTypeService
    {
        Task<IEnumerable<DocumentType>> GetList();
    }
}
