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
    public class PageService : IPageService
    {
        private readonly ScanAppContext _context;

        public PageService() 
        { 
            _context = new ScanAppContext();
        }

        public IEnumerable<Page> GetByDocumentId(long Id)
        {
            return _context.pages.Include(e => e.Images).Where(x => x.Document_Id == Id).ToList();
        }

        public async Task<long> Create(PageCreateRequest request)
        {
            Page page = new Page()
            {
                Name = request.Name,
                Document_Id = request.DocumentId,
                Path = request.Path,
                Icode = request.Icode,
                Number_Order = request.NumberOrder,
            };

            try
            {
                await _context.pages.AddAsync(page);
                await _context.SaveChangesAsync();
                return page.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> CheckExists(string Icode)
        {
            return await _context.pages.AnyAsync(a => a.Icode == Icode);
        }

        public long GetCurrentOrderByDocumentId(long documentId)
        {
            var page = _context.pages.Where(x => x.Document_Id == documentId).OrderBy(x => x.Number_Order).LastOrDefault();

            if (page == null || page.Number_Order == null)
                return 0;

            return (long)page.Number_Order;
        }
    }
}
