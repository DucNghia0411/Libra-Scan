using LIBRA.Scan.Data.EFs;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Services.Constracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly ScanAppContext _context;

        public DocumentTypeService() 
        {
            _context = new ScanAppContext();
        }

        public async Task<IEnumerable<DocumentType>> GetList()
        {
            return  await _context.document_type.ToListAsync();
        }
    }
}
