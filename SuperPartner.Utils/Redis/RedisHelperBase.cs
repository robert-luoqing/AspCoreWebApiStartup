using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperPartner.Utils.Redis
{
    /// <summary>
    /// The class to wrap redis
    /// The benefit is that we can switch difference redis library
    /// </summary>
    public abstract class RedisHelperBase
    {
        /// <summary>
        /// It is redis object
        /// </summary>
        protected ConnectionMultiplexer Redis { get; }
        /// <summary>
        /// It is prefix of redis key.
        /// All operation will add the prefix
        /// like: HashIncrement("test","3333",12);
        /// if the prefix is "edu", the finally key will be "edu:test"
        /// Notice, pipeline operation will be invoke GetFinalRedisKey method to convert key
        /// </summary>
        private string RedisKeyPrefix { get; }

        /// <summary>
        /// Redis Factory
        /// </summary>
        /// <param name="connectString">The format should be "server1:6379,server2:6379"</param>
        /// <param name="prefix">Redis key的前缀</param>
        public RedisHelperBase(string connectString, string prefix)
        {
            this.Redis = ConnectionMultiplexer.Connect(connectString);
            this.RedisKeyPrefix = prefix;
        }

        public string GetFinalRedisKey(string key)
        {
            if (string.IsNullOrWhiteSpace(this.RedisKeyPrefix))
                return key;
            else
                return this.RedisKeyPrefix + ":" + key;
        }

        /// <summary>
        /// Increase value in hash
        /// </summary>
        /// <param name="key">Redis key</param>
        /// <param name="fieldId">The hash list id</param>
        /// <param name="incrementBy">increement by</param>
        /// <returns>the value after changed</returns>
        public double HashIncrement(string key, string fieldId, double incrementBy)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().HashIncrement(key, fieldId, incrementBy);
        }

        /// <summary>
        /// Set hash value
        /// </summary>
        /// <param name="key">Redis key</param>
        /// <param name="fieldId">The id of hash list</param>
        /// <param name="value">Value</param>
        /// <returns> 1 if field is a new field in the hash and value was set. 0 if field already exists
        ///     in the hash and the value was updated.
        /// </returns>
        public bool HashSet(string key, string fieldId, string value)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().HashSet(key, fieldId, value);
        }

        /// <summary>
        /// Get hash value from one item
        /// </summary>
        /// <param name="key">Redis key</param>
        /// <param name="fieldId">The hash list id</param>
        /// <returns>The hash value</returns>
        public string HashGet(string key, string fieldId)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().HashGet(key, fieldId);
        }

        public void ListRightPush(string key, string value)
        {
            key = this.GetFinalRedisKey(key);
            this.Redis.GetDatabase().ListRightPush(key, value);
        }

        public bool SetNX(string key, string value)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().SetAdd(key, value);
        }

        public IEnumerable<string> GetKeysByPattern(string pattern)
        {
            pattern = this.GetFinalRedisKey(pattern);
            var server = this.Redis.GetServer(this.Redis.GetEndPoints()[0]); //默认一个服务器
            var keys = server.Keys(pattern: pattern);
            return keys.Select(o => (string)o).ToList();
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            var batch = this.Redis.GetDatabase().CreateBatch();
            foreach (var key in keys)
            {
                batch.KeyDeleteAsync(this.GetFinalRedisKey(key));
            }

            batch.Execute();
        }

        public bool Set(string key, string value)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().StringSet(key, value);
        }

        public bool Set(string key, string value, TimeSpan expiresIn)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().StringSet(key, value, expiresIn);
        }

        public string[] GetValuesFromHash(string key, params string[] fieldIds)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().HashGet(key, fieldIds.Select(o => (RedisValue)o).ToArray()).ToStringArray();
        }

        public bool KeyExpire(string key, TimeSpan expireIn)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().KeyExpire(key, expireIn);
        }

        public Dictionary<string, string> HashGetAll(string key)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().HashGetAll(key).ToStringDictionary();
        }

        public bool Remove(string key)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().KeyDelete(key);
        }

        public string Get(string key)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().StringGet(key);
        }

        public int GetExpireTime(string key)
        {
            key = this.GetFinalRedisKey(key);
            var time = this.Redis.GetDatabase().StringGetWithExpiry(key);
            var seconds = time.Expiry == null ? 0 : time.Expiry.Value.TotalSeconds;
            return Convert.ToInt32(seconds);
        }

        public string GetItemFromList(string key, int listIndex)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().ListGetByIndex(key, listIndex);
        }

        public long GetListCount(string key)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().ListLength(key);
        }

        public string[] GetRangeFromList(string key, int startingFrom, int endingAt)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().ListRange(key, startingFrom, endingAt).ToStringArray();
        }

        public void SetItemInList(string key, int listIndex, string value)
        {
            key = this.GetFinalRedisKey(key);
            this.Redis.GetDatabase().ListSetByIndex(key, listIndex, value);
        }

        public bool HashDelete(string key, string fieldId)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().HashDelete(key, fieldId);
        }

        public bool ContainsKey(string key)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().KeyExists(key);
        }

        public string[] GetAllItemsFromList(string key)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().ListRange(key).ToStringArray();
        }

        public void ListLeftPush(string key, string value)
        {
            key = this.GetFinalRedisKey(key);
            this.Redis.GetDatabase().ListLeftPush(key, value);
        }

        public string ListRightPop(string key)
        {
            key = this.GetFinalRedisKey(key);
            return this.Redis.GetDatabase().ListRightPop(key);
        }
    }
}
