using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkSumApp.Services
{
    public interface IDataStore<TModel>
        where TModel : class, new()
    {
        Task<List<TModel>> GetItems(Func<TModel, bool> predicate = null);
        Task<TModel> GetItem(object id);
        Task<bool> AddItem(TModel model);
        Task<bool> UpdateItem(TModel model);
        Task<bool> DeleteItem(TModel model);
    }
}
