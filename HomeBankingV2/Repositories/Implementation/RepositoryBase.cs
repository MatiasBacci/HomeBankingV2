using HomeBankingV2.Models;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HomeBankingV2.Repositories.Implementation
{
    //en esta clase RepositoryBase se hace uso del contexto (que maneja finalmente la comunicación y mapeo con la base de datos relacional) para ejecutar las acciones mencionadas.
    //Lo importante de esta implementación base, es que con la abstracción, ahora sólo debemos preocuparnos de ir implementando el patrón de repositorio por
    //cada clase que necesitemos, ya no debemos modificar nuestra implementación base.
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected HomeBankingV2Context RepositoryContext { get; set; }

        public RepositoryBase(HomeBankingV2Context repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll()
        {
            //return this.RepositoryContext.Set<T>().AsNoTracking();
            return this.RepositoryContext.Set<T>().AsNoTrackingWithIdentityResolution();
        }

        public IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            IQueryable<T> queryable = this.RepositoryContext.Set<T>();

            if (includes != null)
            {
                queryable = includes(queryable);
            }
            return queryable.AsNoTrackingWithIdentityResolution();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>().Where(expression).AsNoTrackingWithIdentityResolution();
        }

        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }

        public void SaveChanges()
        {
            this.RepositoryContext.SaveChanges();
        }
    }

}