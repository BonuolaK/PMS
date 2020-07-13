using PMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.Shared.Service
{
    public interface IBaseService<T> where T : BaseEntity
    {
        void Create(T model);

        void Update(T model);

        T Get(int id);

        IEnumerable<T> GetAll();

        IEnumerable<T> GetAll(Expression<Func<T, bool>> expression);

        void Delete(int id);
    }
}
