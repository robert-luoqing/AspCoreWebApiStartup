using SuperPartner.Utils.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace SuperPartner.Utils.Redis
{
    /// <summary>
    /// It used to set permission related information in redis, like token
    /// </summary>
    public class PermissionRedisHelper : RedisHelperBase
    {
        public PermissionRedisHelper(string connectString, string prefix) : base(connectString, prefix)
        {

        }
    }
}
