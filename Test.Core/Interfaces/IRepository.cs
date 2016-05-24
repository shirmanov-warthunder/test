using System.Collections.Generic;
using System.Threading.Tasks;
using Test.Core.Domain.Base;

namespace Test.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        TEntity GetById(int id);

        Task<TEntity> GetByIdAsync(int id);

        void Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        int Save();

        Task<int> SaveAsync();

        void DisposeContext();
    }
}