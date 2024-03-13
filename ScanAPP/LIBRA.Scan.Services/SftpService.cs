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
    public class SftpService : ISftpService
    {
        private readonly ScanAppContext _context;

        public SftpService()
        {
            _context = new ScanAppContext();
        }

        public SftpConfig Get()
        {
           return _context.sftp_config.AsNoTracking().FirstOrDefault();
        }

        public bool Update(SftpUpdateRequest request)
        {
            var config = _context.sftp_config.Find(request.id);

            if (config == null)
                return false;

            config.host_name = request.host_name;
            config.port_number = request.port_number;
            config.user_name = request.user_name;
            config.password = request.password;
            config.host_key = request.host_key;
            config.folder_path = request.folder_path;

            _context.sftp_config.Update(config);

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
