using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Model.Common
{
    /// <summary>
    /// Pager parameter for request
    /// CurrentPage is based 1
    /// </summary>
    public class WsPager
    {
        public WsPager()
        {
            CurrentPage = 1;
            ItemsPerPage = 20;
        }
        
        /// <summary>
        /// The current page which client request. It start from 1
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// Recond count per page
        /// </summary>
        public int ItemsPerPage { get; set; }
    }
}
