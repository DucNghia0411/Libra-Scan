using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IUrlService
    {
        IEnumerable<UrlConfig> GetAll();
        bool Create(UrlCreateRequest request);
    }
}
