using LIBRA.Scan.Data.EFs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Data.InfraStructure
{
    public interface IDatabaseFactory
    {
        ScanAppContext Init();

        void ReNew();
    }

    public class DatabaseFactory : IDatabaseFactory
    {
        private ScanAppContext _context;

        public ScanAppContext Init()
        {
            return _context ?? (_context = new ScanAppContext());
        }

        public void ReNew()
        {
            try
            {
                _context.Dispose();
            }
            catch { };

            _context = new ScanAppContext();
        }
    }
}
