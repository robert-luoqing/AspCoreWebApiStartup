using SuperPartner.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperPartner.DataLayer.Common
{
    public abstract class DaoBase
    {
        public DaoContext DaoContext { get; }
        public DaoBase(DaoContext daoContext)
        {
            this.DaoContext = daoContext;
        }

        /// <summary>
        /// Aplly the pager for fetch list
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="resultQuery">The Query</param>
        /// <param name="pager">The pager data</param>
        /// <returns>The new query object that apply pager</returns>
        protected IQueryable<T> ApplyPager<T>(IQueryable<T> resultQuery, WsPager pager)
        {
            if (pager.CurrentPage <= 1)
            {
                var newQuery = resultQuery.Take(pager.ItemsPerPage);
                return newQuery;
            }
            else
            {
                var newQuery = resultQuery.Skip((pager.CurrentPage - 1) * pager.ItemsPerPage).Take(pager.ItemsPerPage);
                return newQuery;
            }
        }
    }
}
