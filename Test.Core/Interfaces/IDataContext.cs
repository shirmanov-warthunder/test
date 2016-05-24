using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Test.Core.Interfaces
{
    public interface IDataContext
    {
        int SaveChanges();

        Task<int> SaveChangesAsync();

        DbEntityEntry<T> Entry<T>(T entity) where T : class;

        DbSet<T> Set<T>() where T : class;

        void Dispose();
    }
}