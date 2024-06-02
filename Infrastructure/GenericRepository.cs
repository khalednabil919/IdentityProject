using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RoboGas.Core.Interfaces;


namespace RoboGas.Infrastructure.Repositories
{
    public partial class GenericRepository<TEntity>: IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        public GenericRepository(DbContext context)
        {
            _context = context;
        }
        private IEntityType GetEntityType()
        {
            return _context.Model.FindEntityType(typeof(TEntity));
        }
        public string? GetSchema()
        {
            return GetEntityType().GetSchema();
        }
        public string? GetTableName()
        {
            return GetEntityType().GetTableName();
        }
        public string? GetPrimaryKeyColmunName()
        {
            return GetEntityType().GetKeys().Select(p => p.GetName()).FirstOrDefault();
        }
        public DbSet<TEntity> DbSet()
        {
            return _context.Set<TEntity>();
        }
        public IQueryable<TEntity> Query()
        {
            return DbSet().AsQueryable();
        }

        public void SetContextState(TEntity entity, EntityState state)
        {
            _context.Entry(entity).State = state;
        }

        // Sync
        public TEntity Add(TEntity entity)
        {
            DbSet().Add(entity);
            return entity;
        }
        public IEnumerable<TEntity>GetAll()
        {
            return DbSet();
        }
        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            DbSet().AddRange(entities);
            return entities;
        }

        public TEntity Update(TEntity entity)
        {
            _context.Update(entity);
            return entity;
        }
        public IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
        {
            _context.UpdateRange(entities);
            return entities;
        }

        public void Delete(object id)
        {
            var element = DbSet().Find(id);
            Delete(element);
        }
        public void DeleteRange(IEnumerable<object> ids)
        {
            List<TEntity> lTEntity = new List<TEntity>();
            foreach (int id in ids)
                lTEntity.Add(DbSet().Find(id));
            DeleteRange(lTEntity);

        }

        public void Delete(TEntity entity)
        {
            DbSet().Remove(entity);
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            DbSet().RemoveRange(entities);
        }

        public void Attach(TEntity entity)
        {
            DbSet().Attach(entity);
        }
        public void AttachRange(IEnumerable<TEntity> entities)
        {
            DbSet().AttachRange(entities);
        }


        // ASync
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await DbSet().AddAsync(entity);
            return entity;
        }
        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet().AddRangeAsync(entities);
            return entities;
        }

    }
}
