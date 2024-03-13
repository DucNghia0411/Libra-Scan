using LIBRA.Scan.Common.Status;
using LIBRA.Scan.Data.EFs;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.Respones;
using LIBRA.Scan.Services.Constracts;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ScanAppContext _context;

        public DocumentService()
        {
            _context = new ScanAppContext();
        }

        public async Task<long> Create(DocumentCreateRequest request)
        {
            Document document = new Document()
            {
                Name = request.Name,
                Batch_Id = request.BatchId,
                User_Id = request.UserId,
                Administrative_Division = request.AdministrativeDivision,
                Document_No = request.DocumentNo,
                Document_Year = request.DocumentYear,
                Document_Type_Id = request.DocumentTypeId,
                Scanned_Images = request.ScannedImages,
                Note = request.Note,
                Path = request.Path,
                Icode = Guid.NewGuid().ToString(),
                Doc_Process_Status = (long)ProcessStatus.New,
                Doc_Scan_Status = (long)ScanStatus.New
            };

            try
            {
                await _context.document.AddAsync(document);
                await _context.SaveChangesAsync();
                return document.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public IEnumerable<Document> GetDocuments()
        {
            return _context.document
                .Include(e => e.DocumentType)
                .Include(e => e.StatusProcess)
                .Include(e => e.StatusScan).ToList();
        }

        public IEnumerable<Document> GetDocumentsByBatchId(long batchId)
        {
            return _context.document
                .Include(e => e.DocumentType)
                .Include(e => e.StatusProcess)
                .Include(e => e.StatusScan).Where(x => x.Batch_Id == batchId && x.Deleted != 1).AsNoTracking().ToList();
        }

        public long Update(DocumentUpdateRequest request)
        {
            var document = _context.document.Find(request.Id);

            if (document == null)
                return 0;

            document.Name = request.Name;
            document.Administrative_Division = request.AdministrativeDivision;
            document.Document_No = request.DocumentNo;
            document.Document_Year = request.DocumentYear;
            document.Document_Type_Id = request.DocumentTypeId;
            document.Scanned_Images = request.ScannedImages;
            document.Note = request.Note;

            _context.document.Update(document);

            try
            {
                _context.SaveChanges();
                return document.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool Delete(long id)
        {
            var document = _context.document.Find(id);

            if (document == null)
                return false;

            document.Deleted = 1;

            _context.document.Update(document);

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
