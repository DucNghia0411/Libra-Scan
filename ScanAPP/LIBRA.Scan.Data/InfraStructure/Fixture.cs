using Docnet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LIBRA.Scan.Data.InfraStructure
{
    public class Fixture : IDisposable
    {
        public IDocLib DocNet { get; }

        public Fixture()
        {
            DocNet = DocLib.Instance;
        }

        public void Dispose()
        {
            DocNet.Dispose();
        }

        [CollectionDefinition("FixtureCollection")]
        public class FixtureCollection : ICollectionFixture<Fixture> { }
    }
}
