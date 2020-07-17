using PMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.Shared.Service
{
    public interface IBaseService<TCreateUpdateDto,TEntity, TEntityDto> where TEntity  : BaseEntity
    {
        ServiceResultModel<TEntity> Create(TCreateUpdateDto model);

        ServiceResultModel<TEntity> Update(TCreateUpdateDto model);


        TEntity Get(int id);

        TEntity Get(Expression<Func<TEntity, bool>> expression);

        IEnumerable<TEntityDto> GetAll();



        IEnumerable<TEntityDto> GetAll(Expression<Func<TEntity, bool>> expression);

        ServiceResultModel<int> Delete(int id);
    }
}
