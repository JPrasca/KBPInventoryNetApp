using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using PagedList;

namespace Repository
{
    public interface IRepository: IDisposable
    {
        T Create<T>(T newEntity) where T : class;
        bool Update<T>(T updateEntity) where T : class;
        bool Delete<T>(T deleteEntity) where T : class;
        T Find<T>(Expression<Func<T, bool>> expression) where T : class;
        IEnumerable<T> FindSet<T>(Expression<Func<T, bool>> expression) where T : class;
        IPagedList<T> FindSetPage<T>(Expression<Func<T, bool>> expression, Expression<Func<T, string>> order, int page, int pageSize) where T : class;


    }
}
