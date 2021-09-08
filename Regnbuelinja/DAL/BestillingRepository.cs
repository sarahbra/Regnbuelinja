using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.DAL
{
    public class BestillingRepository
    {
        private readonly BestillingContext _db;

        public BestillingRepository(BestillingContext db)
        {
            _db = db;
        }

        public async Task<int> LagreBestilling()
        {
            return 0;
        }
    }
}
