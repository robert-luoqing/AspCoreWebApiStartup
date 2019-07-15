using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Model.Exception
{
    /// <summary>
    /// Author: Robert
    /// Business Exception. The default error code is 8888 if no error code setted
    /// Notice: Business Exception is used to notice what error occur. It is predictable.
    /// </summary>
    public class SpException : System.Exception
    {
        public string ErrorCode { get; set; }
        public SpException(string errorCode, string errorMessage) : base(errorMessage)
        {
            this.ErrorCode = errorCode;
        }

        public SpException(string errorMessage) : base(errorMessage)
        {
            this.ErrorCode = "8888";
        }
    }
}
