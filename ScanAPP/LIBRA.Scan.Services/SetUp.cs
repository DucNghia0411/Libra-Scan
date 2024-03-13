using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LIBRA.Scan.Services
{
    public interface ISetUp
    {
        TwainSession GetTwainSession();
        IEnumerable<DataSource> GetDataSource();
        TwainSession GetPresentTwainSession();
    }
    public class SetUp : ISetUp
    {
        public TwainSession? _twain;
        public DataSource? _dataSource;

        public TwainSession GetTwainSession()
        {
            _twain = new TwainSession(TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetExecutingAssembly()));
            _twain.Open();
            return _twain;
        }
        
        public IEnumerable<DataSource> GetDataSource()
        {
            var sources = _twain.GetSources();
            return sources;
        }

        public TwainSession GetPresentTwainSession()
        {
            return _twain;
        }
    }
}
