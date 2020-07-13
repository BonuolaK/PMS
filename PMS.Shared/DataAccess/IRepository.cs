using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PMS.Shared.DataAccess
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] propertySelectors);
        IList<T> List();
        IList<T> List(Expression<Func<T, bool>> expression);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
