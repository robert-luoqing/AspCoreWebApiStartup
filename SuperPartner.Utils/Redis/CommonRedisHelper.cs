using Microsoft.Extensions.Options;

namespace SuperPartner.Utils.Redis
{
    /// <summary>
    /// It is common redis object. If no specify information was given, the data will be save the redis
    /// </summary>
    public class CommonRedisHelper : RedisHelperBase
    {
        public CommonRedisHelper(string connectString, string prefix) : base(connectString, prefix)
        {
            
        }
    }
}
