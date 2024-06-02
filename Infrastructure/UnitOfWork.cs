using Core.Auth;
using Core.Entities;
using DataTransferObject.Entity;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RoboGas.Core.Interfaces;

namespace RoboGas.Infrastructure.Repositories
{
    public class UnitOfWork: IUnitOfWork 
    {
        private readonly DbContext _context;

        public IGenericRepository<User> user { get; set; }
        public IGenericRepository<Region> region { get; set; }
        public IGenericRepository<RefreshTokenDevCreed> refreshToken { get; set; }
        public UnitOfWork(DataAccess context) 
        {
            _context = context;

            user = new GenericRepository<User>(_context);
            region = new GenericRepository<Region>(_context);
            refreshToken = new GenericRepository<RefreshTokenDevCreed>(_context);
        }

        public IDbContextTransaction Transaction()
        {
            return _context.Database.BeginTransaction();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
