using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkSumApp.Services
{
    class EFDataStore<TModel> : IDataStore<TModel>
        where TModel : class, new()
    {
        private readonly DbContext _context;
        public EFDataStore(DbContext context)
        {
            _context = context;
        }


        #region IDataStore
        Task<bool> IDataStore<TModel>.AddItem(TModel model)
        {
            return Task.Run(() =>
            {

                try
                {
                    _context.Set<TModel>().Add(model);
                    return _context.SaveChanges() > 0;
                }
                catch (Exception e)
                {

                }
                return false;
            });
        }

        Task<bool> IDataStore<TModel>.DeleteItem(TModel model)
        {
            return Task.Run(() =>
            {

                try
                {
                    _context.Entry(model).State = EntityState.Deleted;
                    return _context.SaveChanges() > 0;
                }
                catch (Exception e)
                {

                }
                return false;
            });
        }

        Task<TModel> IDataStore<TModel>.GetItem(object id)
        {
            return Task.Run(() =>
            {

                try
                {
                    return _context.Set<TModel>().Find(id);
                }
                catch (Exception e)
                {

                }
                return null;
            });
        }

        Task<List<TModel>> IDataStore<TModel>.GetItems(Func<TModel, bool> predicate = null)
        {
            return Task.Run(() =>
            {

                try
                {
                    if (predicate != null)
                        return _context.Set<TModel>().Where(predicate).ToList();
                    return _context.Set<TModel>().ToList();
                }
                catch (Exception e)
                {

                }
                return null;
            });
        }
        Task<bool> IDataStore<TModel>.UpdateItem(TModel model)
        {
            return Task.Run(() =>
            {

                try
                {
                    _context.Entry(model).State = EntityState.Modified;
                    return _context.SaveChanges() > 0;
                }
                catch (Exception e)
                {

                }
                return false;
            });
        }
        #endregion

    }
}
