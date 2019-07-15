using SuperPartner.DataLayer.DataContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.DataLayer.Common
{
    /// <summary>
    /// This is dao context which serve datalayer
    /// </summary>
    public class DaoContext
    {
        private SpFrameworkContext databaseContext;
        public DaoContext(SpFrameworkContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <summary>
        /// Get database context
        /// </summary>
        /// <returns></returns>
        public SpFrameworkContext GetDbContext()
        {
            return this.databaseContext;
        }
    }
}
