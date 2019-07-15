using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Model.Common
{
    /// <summary>
    /// Author: Robert
    /// The response base class
    /// All response class will be use the class as base class
    /// </summary>
    public class WsResponse
    {
        public WsResponse()
        {
            this.Trans = new WsTrans()
            {
                ErrorCode = "0"
            };
        }

        /// <summary>
        /// Response trans object
        /// </summary>
        public WsTrans Trans { get; set; }
    }

    /// <summary>
    /// The response class which include data
    /// </summary>
    /// <typeparam name="T">The response data type</typeparam>
    public class WsResponse<T> : WsResponse
    {
        public T Data { get; set; }

        /// <summary>
        /// Implicit convert T to WsResponse<T>
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator WsResponse<T>(T value)
        {
            var result = new WsResponse<T>();
            result.Data = value;
            return result;
        }
    }
}
