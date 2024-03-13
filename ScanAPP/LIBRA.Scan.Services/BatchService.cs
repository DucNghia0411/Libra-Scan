using LIBRA.Scan.Common.Status;
using LIBRA.Scan.Data.EFs;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Services.Constracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services
{
    public class BatchService : IBatchService
    {
        private readonly ScanAppContext _context;

        public BatchService()
        {
            _context = new ScanAppContext();
        }

        public async Task<long> Create(BatchCreateRequest request)
        {
            Batch batch = new Batch()
            {
                Name = request.Name,
                User_Id = request.UserId,
                Note = request.Note,
                Path = request.Path,
                Status = (long)BatchStatus.New,
                Created_Date = request.CreatedDate,
                Icode = request.Icode
            };

            try
            {
                await _context.batch.AddAsync(batch);
                await _context.SaveChangesAsync();
                return batch.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public IEnumerable<Batch> GetAll()
        {
            return _context.batch.Include(e => e.StatusBatch).Where(x => x.Deleted != 1).ToList();
        }

        public IEnumerable<Batch> GetBatchesByUserId(long userId)
        {
            return _context.batch.Include(e => e.StatusBatch).Where(x => x.User_Id == userId && x.Deleted != 1).AsNoTracking().ToList();
        }

        public bool Update(BatchUpdateRequest request)
        {
            var batch = _context.batch.Find(request.Id);

            if (batch == null)
                return false;

            batch.Name = request.Name;
            batch.Note = request.Note;

            _context.batch.Update(batch);

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

        public bool Delete(long id)
        {
            var batch = _context.batch.Find(id);

            if (batch == null)
                return false;

            batch.Deleted = 1;

            _context.batch.Update(batch);

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
