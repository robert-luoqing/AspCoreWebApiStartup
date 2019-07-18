using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Model.Common
{
    public class SpConfiguration
    {
        /// <summary>
        /// Permission redis connection string
        /// </summary>
        public string PermissionRedis { get; set; }
        /// <summary>
        /// The whole app's redis prefix
        /// The benefit can use one redis for multiple project. not concern the duplicate keys
        /// </summary>
        public string RedisPrefix { get; set; }
        /// <summary>
        /// Common Redis connection string
        /// </summary>
        public string CommonRedis { get; set; }
        /// <summary>
        /// The interval of password expired
        /// The unit: Day
        /// </summary>
        public int PwdExpiredInterval { get; set; }
        /// <summary>
        /// The max try times of login
        /// null will be infinite times of try
        /// </summary>
        public int? MaxLoginTryTimes { get; set; }
    }
}
