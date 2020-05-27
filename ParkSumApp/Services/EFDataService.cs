using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkSumApp.Services
{
    class EFDataService : IDataService
    {
        private readonly DbContext _context;
        public EFDataService()
        {
            _context = new Models.YF_PCY_DUAL_LGFY_EEntities();
        }

        #region IDataService
        IDataStore<TModel> IDataService.GetDataStore<TModel>()
        {
            return new EFDataStore<TModel>(_context);
        }
        #endregion
    }
}
