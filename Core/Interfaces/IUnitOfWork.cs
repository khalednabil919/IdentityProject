

using Core.Auth;
using Core.Entities;
using DataTransferObject.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace RoboGas.Core.Interfaces
{
    public interface IUnitOfWork :  IDisposable
    {
        public IGenericRepository<User> user { get; set; }
        public IGenericRepository<Region> region { get; set; }
        public IGenericRepository<RefreshTokenDevCreed> refreshToken { get; set; }
        public int Complete();
        public IDbContextTransaction Transaction();
    }
}
