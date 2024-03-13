using LIBRA.Scan.API.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service.Constracts
{
    public interface IHistoryService
    {
        Task<long> Create(HistoryCreateRequest request);
    }
}
