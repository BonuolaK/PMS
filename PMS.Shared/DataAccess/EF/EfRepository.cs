using Microsoft.EntityFrameworkCore;
using PMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PMS.Shared.DataAccess.EF
{
    public class EFRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly BaseContext _dataContext;

        public EFRepository(BaseContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Delete(T entity)
        {
            _dataContext.Set<T>().Remove(entity);
            _dataContext.SaveChanges();
        }

        public T Get(int id)
        {
            return _dataContext.Set<T>().Find(id);
        }



        public T Get(Expression<Func<T, bool>> filter)
        {
            return _dataContext.Set<T>().Where(filter).FirstOrDefault();
        }

        public void Insert(T entity)
        {
            _dataContext.Set<T>().Add(entity);
            _dataContext.SaveChanges();
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] propertySelectors)
        {
            var query = _dataContext.Set<T>().AsQueryable();


            if (propertySelectors.Count() > 0)
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }

            return query;
        }

        public IList<T> List()
        {
            return _dataContext.Set<T>().ToList();
        }

        public IList<T> List(Expression<Func<T, bool>> expression)
        {
            return _dataContext.Set<T>().Where(expression).ToList();
        }

        public void Update(T entity)
        {
            _dataContext.Entry<T>(entity).State = EntityState.Modified;
            _dataContext.SaveChanges();
        }
    }
}
