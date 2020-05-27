using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkSumApp.Services
{
    public interface IDataService
    {
        IDataStore<TModel> GetDataStore<TModel>() where TModel : class, new();
    }
}
