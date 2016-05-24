using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Test.Core.Domain.Base;
using Test.Core.Interfaces;

namespace Test.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IDataContext context;

        private DbSet<TEntity> CurrentSet
        {
            get { return this.context.Set<TEntity>(); }
        }

        public Repository(IDataContext context)
        {
            this.context = context;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this.CurrentSet.AsEnumerable();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await this.CurrentSet.ToListAsync();
        }

        public TEntity GetById(int id)
        {
            return this.CurrentSet.FirstOrDefault(m => m.Id == id);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await this.CurrentSet.FindAsync(id);
        }

        public void Create(TEntity entity)
        {
            this.CurrentSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            this.CurrentSet.Attach(entity);
            this.context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            this.CurrentSet.Remove(entity);
        }

        public int Save()
        {
            return this.context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await this.context.SaveChangesAsync();
        }

        public void DisposeContext()
        {
            this.context.Dispose();
        }
    }
}