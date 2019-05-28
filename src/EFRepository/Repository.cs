using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using PagedList;

namespace EFRepository
{
    public class Repository : IRepository, IDisposable
    {
        protected DbContext Context;
        public Repository(DbContext context, bool autoDetectChangesEnabled=false,bool proxiCreationEnabled = false)
        {
            this.Context = context;
            this.Context.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
            this.Context.Configuration.ProxyCreationEnabled = proxiCreationEnabled;
        }
        public T Create<T>(T newEntity) where T : class
        {
            T Result = null;
            try
            {
                Result = Context.Set<T>().Add(newEntity);
                TrySaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string strError = "";
                foreach(var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach(var validationError in validationErrors.ValidationErrors)
                    {
                        strError += string.Format("Prperty:{0} Error{1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
            }
            return Result;
        }
        protected virtual int TrySaveChanges()
        {
            return Context.SaveChanges();
        }
        public bool Delete<T>(T deleteEntity) where T : class
        {
            bool Result = false;
            try
            {
                Context.Set<T>().Attach(deleteEntity);
                Context.Set<T>().Remove(deleteEntity);
                Result=TrySaveChanges()>0;
            }
            catch (DbEntityValidationException dbEx)
            {
                string strError = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        strError += string.Format("Prperty:{0} Error{1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
            }
            return Result;
        }
        public void Dispose()
        {
            if (Context != null) { Context.Dispose(); }
            throw new NotImplementedException();
        }
         public bool Update<T>(T updateEntity) where T : class
        {
            bool Result = false;
            try
            {
                Context.Set<T>().Attach(updateEntity);
                Context.Entry<T>(updateEntity).State=EntityState.Modified;
                Result = TrySaveChanges() > 0;
            }
            catch (DbEntityValidationException dbEx)
            {
                string strError = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        strError += string.Format("Prperty:{0} Error{1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
            }
            return Result;
        }
        public T Find<T>(Expression<Func<T, bool>> expression) where T : class
        {
            T Result = null;
            try
            {
                Result = Context.Set<T>().FirstOrDefault(expression);
            }
            catch (DbEntityValidationException dbEx)
            {
                string strError = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        strError += string.Format("Prperty:{0} Error{1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
            }
            return Result;
        }
        public IEnumerable<T> FindSet<T>(Expression<Func<T, bool>> expression) where T : class
        {
            IEnumerable<T> Result = null;
            try
            {
                Result = Context.Set<T>().Where(expression).ToList();
            }
            catch (DbEntityValidationException dbEx)
            {
                string strError = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        strError += string.Format("Prperty:{0} Error{1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
            }
            return Result;
        }
        public IPagedList<T> FindSetPage<T>(Expression<Func<T, bool>> expression, Expression<Func<T, string>> order, int page, int pageSize) where T : class
        {
            PagedList<T> Result = null;
            try
            {
                var registrosActuales = Context.Set<T>().Where(expression).OrderBy(order).Select(p => p).ToPagedList(page, pageSize);
                Result = (PagedList<T>)Convert.ChangeType(registrosActuales, typeof(PagedList<T>));
            }
            catch (DbEntityValidationException dbEx)
            {
                string strError = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        strError += string.Format("Prperty:{0} Error{1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
            }
            return Result;
        }
    }
}
