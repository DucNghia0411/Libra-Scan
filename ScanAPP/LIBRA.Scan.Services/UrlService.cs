using LIBRA.Scan.Data.EFs;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Services.Constracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services
{
    public class URLService : IUrlService
    {
        private readonly ScanAppContext _context;

        public URLService()
        {
            _context = new ScanAppContext();
        }

        public IEnumerable<UrlConfig> GetAll()
        {
            return _context.url_config.ToList();
        }

        public bool Create(UrlCreateRequest request)
        {
            UrlConfig config = new UrlConfig()
            {
                scheme = request.scheme,
                host_name = request.host_name,
                port = request.port,
                has_port = request.has_port,
            };

            try
            {
                _context.url_config.Add(config);
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
