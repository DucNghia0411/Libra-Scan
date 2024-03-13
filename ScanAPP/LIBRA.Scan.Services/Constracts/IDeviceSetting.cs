using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IDeviceSettingService
    {
        DeviceSetting GetLatest();
        Task<long> Create(CreateDeviceSettingRequest request);
        bool Update(UpdateDeviceSettingRequest request);
        bool Delete(long id);
    }
}
