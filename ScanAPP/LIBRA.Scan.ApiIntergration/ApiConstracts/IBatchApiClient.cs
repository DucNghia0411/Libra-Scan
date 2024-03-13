using LIBRA.Scan.Entities.Entities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.Respones;
using LIBRA.Scan.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.ApiIntergration.ApiConstracts
{
    public interface IBatchApiClient
    {
        Task<RequestResponse> Create(BatchCreateRequest request);
        Task<List<BatchVM>> GetAll();
    }
}
