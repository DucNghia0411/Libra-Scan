using LIBRA.Scan.API.Data.EFs;
using LIBRA.Scan.API.Data.Repositories;
using LIBRA.Scan.API.Data.Repositories.Constracts;
using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service.Constracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service
{
    public class PageService : IPageService
    {
        private readonly ScanAppContext _context;
        private readonly IPageRepo _pageRepo;

        public PageService
        (
            ScanAppContext context
            , IPageRepo pageRepo
        )
        {
            this._context = context;
            this._pageRepo = pageRepo;
        }

        public async Task<long> Create(PageCreateRequest request)
        {
            Page page = new Page()
            {
                Name = request.Name,
                DocumentId =  request.DocumentId,
                Path = request.Path,
                NumberOrder =request.NumberOrder 
            };
            try
            {
                await _context.Pages.AddAsync(page);
                await _context.SaveChangesAsync();
                return page.Id;
            }
            catch (Exception)
            {
                return (long)Code.InternalServerError;
            }
        }
        public async Task<IEnumerable<Page>> Get(Expression<Func<Page, bool>> predicate)
        {
            IEnumerable<Page> pages = await _pageRepo.ListAsync(predicate);
            return pages;
        }
    }
}
