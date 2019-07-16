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
        //contexto para los datos que provienen de la base de datos
        protected DbContext Context;
        public Repository(DbContext context, bool autoDetectChangesEnabled=false,bool proxiCreationEnabled = false)
        {
            this.Context = context;
            this.Context.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
            this.Context.Configuration.ProxyCreationEnabled = proxiCreationEnabled;
        }
        
        //método para insertar nuevos registros a las tabla
        public T Create<T>(T newEntity) where T : class
        {
            T Result = null;
            try
            {
                //añade el registro
                Result = Context.Set<T>().Add(newEntity);
                TrySaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                //captura el error
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
        
        //guardar los cambios de los datos en la aplicación
        protected virtual int TrySaveChanges()
        {
            return Context.SaveChanges();
        }
        
        //Método para eliminar un registro
        public bool Delete<T>(T deleteEntity) where T : class
        {
            bool Result = false;
            try
            {
                //borrado
                Context.Set<T>().Attach(deleteEntity);
                Context.Set<T>().Remove(deleteEntity);
                Result=TrySaveChanges()>0;
            }
            catch (DbEntityValidationException dbEx)
            {
                //captura error
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
        
        //Para actualizar registros en la BD
         public bool Update<T>(T updateEntity) where T : class
        {
            bool Result = false;
            try
            {
                //actualiza
                Context.Set<T>().Attach(updateEntity);
                Context.Entry<T>(updateEntity).State=EntityState.Modified;
                Result = TrySaveChanges() > 0;
            }
            catch (DbEntityValidationException dbEx)
            {
                //captura el error
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
        
        //Método para el select de un registro desde la BD
        public T Find<T>(Expression<Func<T, bool>> expression) where T : class
        {
            T Result = null;
            try
            {
                //ejecuta la consulta
                Result = Context.Set<T>().FirstOrDefault(expression);
            }
            catch (DbEntityValidationException dbEx)
            {
                //captura el error
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
        
        //Método para el select de varios registros desde la BD
        public IEnumerable<T> FindSet<T>(Expression<Func<T, bool>> expression) where T : class
        {
            //los registros que se van a retornar
            IEnumerable<T> Result = null;
            try
            {
                //ejecuta la consulta
                Result = Context.Set<T>().Where(expression).ToList();
            }
            catch (DbEntityValidationException dbEx)
            {
                //captura el error
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
        
        //Método para seleccionar registros por lote... esto sirve para paginar en una tabla en la aplicación
        public IPagedList<T> FindSetPage<T>(Expression<Func<T, bool>> expression, Expression<Func<T, string>> order, int page, int pageSize) where T : class
        {
            //a retornar
            PagedList<T> Result = null;
            try
            {
                //captura los registros que se mostrarán
                var registrosActuales = Context.Set<T>().Where(expression).OrderBy(order).Select(p => p).ToPagedList(page, pageSize);
                Result = (PagedList<T>)Convert.ChangeType(registrosActuales, typeof(PagedList<T>));
            }
            catch (DbEntityValidationException dbEx)
            {
                //captura el error
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
