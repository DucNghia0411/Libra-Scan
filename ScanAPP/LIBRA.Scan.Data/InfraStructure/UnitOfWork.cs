using LIBRA.Scan.Data.EFs;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Data.InfraStructure
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContextTransaction CreateTransaction();

        void Rollback();

        void Save();

        void Commit();
    }
    public class UnitOfWork : IUnitOfWork
    {
        private ScanAppContext _context;
        private IDatabaseFactory _databaseFactory;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        public UnitOfWork(IDatabaseFactory databaseFactory)
        {
            this._databaseFactory = databaseFactory;
        }

        public ScanAppContext dbContext
        {
            get { return _context ?? (_context = _databaseFactory.Init()); }
            set { _context = value; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();
            _disposed = true;
        }

        public IDbContextTransaction CreateTransaction()
        {
            if (_transaction == null)
                _transaction = dbContext.Database.BeginTransaction();
            else
            {
                _transaction = dbContext.Database.CurrentTransaction;
            }
            return _transaction;
        }

        public void Rollback()
        {
            if (_transaction != null)
                _transaction.Rollback();
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public void Commit()
        {
            if (_transaction != null)
                _transaction.Commit();
        }
    }
}
