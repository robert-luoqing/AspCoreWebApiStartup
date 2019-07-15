using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Model.Common
{
    public class SpConfiguration
    {
        public string PermissionRedis { get; set; }
        public string RedisPrefix { get; set; }
        public string CommonRedis { get; set; }
        /// <summary>
        /// 密码过期时间间隔
        /// </summary>
        public int PwdExpiredInterval { get; set; }
    }
}
