using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFRepository
{
    //implementando las interfaces y el repositorio
    public class RepositoryUoW : Repository, IRepository, IUnitOfWork, IDisposable
    {
        public RepositoryUoW(DbContext context, bool autoDetectChangesEnabled = false, bool proxiCreationEnabled = false):
            base(context, autoDetectChangesEnabled, proxiCreationEnabled)
        {
        }
        
        //guardar cambios
        public int Save()
        {
            int Result = 0;
            try
            {   //guarda
                Result = Context.SaveChanges();
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

        //guardar cambios
        protected override int TrySaveChanges()
        {
            return 0;
        }
    }
}
