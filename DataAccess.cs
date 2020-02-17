using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryIT
{
    public class EmployeeDb : DbContext
    {
        public DbSet<Employee> Employees;
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees");
        }
    }

    public interface IReadOnlyRepository<out T> : IDisposable
    {
        T FindById(int id);
        IQueryable<T> FindAll();
    }

    public interface IWriteOnlyRepository<in T>: IDisposable
    {
        void Add(T newEntity);
        void Delete(T entity);
        int Commit();
    }

    public interface IRepository<T> : IDisposable, IReadOnlyRepository<T>, IWriteOnlyRepository<T>
    {    }

    public class SqlRepository<T> : IRepository<T> where T : class, IEntity
    {
        DbContext _context;
        DbSet<T> _set;
        public SqlRepository(DbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public void Add(T newEntity)
        {
            if (newEntity.IsValid())
            {
                _set.Add(newEntity);
            }
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IQueryable<T> FindAll()
        {
            return _set;
        }

        public T FindById(int id)
        {
            return _set.Find(id);
        }
    }
}
