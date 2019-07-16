using SuperPartner.Utils.Json;
using SuperPartner.Utils.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.TokenHandler
{
    /// <summary>
    /// Author: Robert
    /// The class is used to handle token in Redis
    /// </summary>
    public class RedisTokenHandler : ITokenHandler
    {
        /// <summary>
        /// Redis helper which use to save or query redis
        /// </summary>
        private RedisHelperBase redisHelper;

        /// <summary>
        /// This is redis key's prefix
        /// </summary>
        private string prefix;

        /// <summary>
        /// The timeout
        /// default timeout is 30*60(30min)
        /// The unit: second
        /// </summary>
        private int timeout = 1800;

        public RedisTokenHandler(RedisHelperBase redisHelper, string prefix)
        {
            this.redisHelper = redisHelper;
            this.prefix = prefix;
        }

        /// <summary>
        /// The method is genarate token which associate with a object
        /// </summary>
        /// <param name="obj">the object which associate with the token</param>
        /// <returns>return a token</returns>
        public virtual string GeneToken(object obj)
        {
            string token = Guid.NewGuid().ToString("N");
            var redisKey = GetRedisKey(token);
            if (obj != null)
                redisHelper.Set(redisKey, JsonHelper.ToJson(obj, true), new TimeSpan(0, 0, this.timeout));
            else
                redisHelper.Set(redisKey, "", new TimeSpan(0, 0, this.timeout));

            return token;
        }

        /// <summary>
        /// update token expired time
        /// basically, it will be invoke in Authorization Filter
        /// </summary>
        /// <param name="token">token</param>
        public virtual void RefreshToken(string token)
        {
            var redisKey = GetRedisKey(token);
            this.redisHelper.KeyExpire(redisKey, new TimeSpan(0, 0, this.timeout));
        }

        /// <summary>
        /// Remove the token
        /// </summary>
        /// <param name="token">token</param>
        public void RemoveToken(string token)
        {
            var redisKey = GetRedisKey(token);
            this.redisHelper.Remove(redisKey);
        }

        /// <summary>
        /// Update the associate object by token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="obj">The associate object</param>
        public virtual void UpdateAssociateDataByToken(string token, object obj)
        {
            var redisKey = GetRedisKey(token);
            string str = "";
            if (obj != null)
                str = JsonHelper.ToJson(obj, true);

            redisHelper.Set(redisKey, str, new TimeSpan(0, 0, this.timeout));
        }

        /// <summary>
        /// Get the associate object by token
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="token">token</param>
        /// <returns>the assicate object will return if the token exist, the null will be return if the token didn't exist</returns>
        public virtual T GetAssociateObjectByToken<T>(string token)
        {
            var redisKey = GetRedisKey(token);
            var jsonStr = this.redisHelper.Get(redisKey);
            if (string.IsNullOrEmpty(jsonStr))
            {
                return default(T);
            }
            else
            {
                return JsonHelper.ToObj<T>(jsonStr, true);
            }
        }

        /// <summary>
        /// Get timeout of token
        /// the unit is second
        /// </summary>
        /// <returns>return the timeout</returns>
        public virtual int GetTimeout()
        {
            return this.timeout;
        }

        /// <summary>
        /// Set the token timeout
        /// the unit is second
        /// </summary>
        /// <param name="timespan"></param>
        public virtual void SetTimeout(int timespan)
        {
            this.timeout = timespan;
        }

        /// <summary>
        /// Generate redis key with prefix and token
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>The redis key combine the prefix and token</returns>
        private string GetRedisKey(string token)
        {
            return this.prefix + ":" + token;
        }
    }
}
