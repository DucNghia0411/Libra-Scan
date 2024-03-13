using LIBRA.Scan.API.Data.EFs;
using LIBRA.Scan.API.Data.Repositories;
using LIBRA.Scan.API.Data.Repositories.Constracts;
using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service
{
    public class HistoryService : IHistoryService
    {
        private readonly ScanAppContext _context;
        private readonly IHistoryRepo _historyRepo;

        public HistoryService
        (
            ScanAppContext context
            , IHistoryRepo historyRepo
        ) 
        {
            this._context = context;
            this._historyRepo = historyRepo;
        }

        public async Task<long> Create(HistoryCreateRequest request)
        {
            History history = new History()
            {
                DocumentId = request.DocumentId,
                PageId = request.PageId,
                Note = request.Note,
                Actions = request.Actions,
                CreatedDate = request.CreatedDate,
                UserId = request.UserId,
                ImageId = request.ImageId 
            };
            try
            {
                await _context.Histories.AddAsync(history);
                await _context.SaveChangesAsync();
                return history.Id;
            }
            catch (Exception)
            {
                return (long)Code.InternalServerError;
            }
        }
    }
}
