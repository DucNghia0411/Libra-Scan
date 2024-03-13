using Azure.Core;
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
    public class DeviceSettingService : IDeviceSettingService
    {
        private readonly ScanAppContext _context;

        public DeviceSettingService()
        {
            _context = new ScanAppContext();
        }

        public DeviceSetting GetLatest()
        {
            var setting = _context.device_setting.AsNoTracking().OrderBy(x => x.id).LastOrDefault();
            return setting;
        }

        public async Task<long> Create(CreateDeviceSettingRequest request)
        {
            DeviceSetting deviceSetting = new DeviceSetting()
            {
                device_manufacturer = request.manufacturer,
                device_name = request.device_name,
                depth = request.depth,
                dpi = request.dpi,
                size = request.size,
                duplex = request.duplex
            };

            try
            {
                await _context.device_setting.AddAsync(deviceSetting);
                await _context.SaveChangesAsync();
                return deviceSetting.id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool Update(UpdateDeviceSettingRequest request)
        {
            var deviceSetting = _context.device_setting.Find(request.id);

            if (deviceSetting == null)
                return false;

            deviceSetting.depth = request.depth;
            deviceSetting.dpi = request.dpi;
            deviceSetting.size = request.size;
            deviceSetting.duplex = request.duplex;


            _context.device_setting.Update(deviceSetting);

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
            var deviceSetting = _context.device_setting.Find(id);

            if (deviceSetting == null)
                return false;

            _context.device_setting.Remove(deviceSetting);

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
