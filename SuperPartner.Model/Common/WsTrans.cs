using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Model.Common
{
    /// <summary>
    /// Auth: Robert
    /// Trans object, It indicate the operation status
    /// </summary>
    public class WsTrans
    {
        /// <summary>
        /// The operation is successful if Error Code is null or zero
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// If there is exception exist, the field is the error message
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
