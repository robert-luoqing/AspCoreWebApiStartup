using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.TokenHandler
{
    /// <summary>
    /// Author: Robert
    /// The class is used to handle token in memory
    /// </summary>
    public class MemoryTokenHandler : ITokenHandler
    {
        /// <summary>
        /// The memory token data structure
        /// </summary>
        public class MemoryTokenData
        {
            /// <summary>
            /// The last access time
            /// </summary>
            public DateTime LastAccessTime { get; set; }
            /// <summary>
            /// The data which associate with token
            /// </summary>
            public object AssociateData { get; set; }
        }

        /// <summary>
        /// The object is used to save token and associate data
        /// </summary>
        private Dictionary<string, MemoryTokenData> tokens = new Dictionary<string, MemoryTokenData>();

        /// <summary>
        /// The timeout
        /// default timeout is 30*60(30min)
        /// </summary>
        private int timeout = 1800;

        /// <summary>
        /// The method is genarate token which associate with a object
        /// </summary>
        /// <param name="obj">the object which associate with the token</param>
        /// <returns>return a token</returns>
        public virtual string GeneToken(object obj)
        {
            string token = Guid.NewGuid().ToString("N");

            var tokenData = new MemoryTokenData();
            tokenData.LastAccessTime = DateTime.Now;
            tokenData.AssociateData = obj;
            // because the Dictionary type is not thread safe object, so use lock to ensure thread safe
            lock (this.tokens)
            {
                this.tokens.Add(token, tokenData);
            }

            return token;
        }

        /// <summary>
        /// update token expired time
        /// basically, it will be invoke in Authorization Filter
        /// </summary>
        /// <param name="token">token</param>
        public virtual void RefreshToken(string token)
        {
            MemoryTokenData tokenData = this.GetAssociateData(token);

            if (tokenData != null)
            {
                // If the token didn't not expired. refresh the last access time
                // else remove the token from tokens
                if (tokenData.LastAccessTime.AddSeconds(this.timeout) >= DateTime.Now)
                {
                    tokenData.LastAccessTime = DateTime.Now;
                }
                else
                {
                    this.RemoveToken(token);
                }
            }
        }

        /// <summary>
        /// Remove the token
        /// </summary>
        /// <param name="token">token</param>
        public virtual void RemoveToken(string token)
        {
            lock (this.tokens)
            {
                this.tokens.Remove(token);
            }
        }

        /// <summary>
        /// Update the associate object by token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="obj">The associate object</param>
        public virtual void UpdateAssociateDataByToken(string token, object obj)
        {
            MemoryTokenData tokenData = this.GetAssociateData(token);

            if (tokenData != null)
            {
                // If the token didn't not expired. refresh the last access time and associate data
                // else remove the token from tokens
                if (tokenData.LastAccessTime.AddSeconds(this.timeout) >= DateTime.Now)
                {
                    tokenData.LastAccessTime = DateTime.Now;
                    tokenData.AssociateData = obj;
                }
                else
                {
                    this.RemoveToken(token);
                }
            }
        }

        /// <summary>
        /// Get the associate object by token
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="token">token</param>
        /// <returns>the assicate object will return if the token exist, the null will be return if the token didn't exist</returns>
        public virtual T GetAssociateObjectByToken<T>(string token)
        {
            T result = default(T);
            MemoryTokenData tokenData = this.GetAssociateData(token);
            if (tokenData != null)
            {
                // If the token didn't not expired. refresh the last access time and associate data
                // else remove the token from tokens
                if (tokenData.LastAccessTime.AddSeconds(this.timeout) >= DateTime.Now)
                {
                    if (tokenData.AssociateData != null)
                        result = (T)tokenData.AssociateData;
                }
                else
                {
                    this.RemoveToken(token);
                }
            }

            return result;
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
        public void SetTimeout(int timespan)
        {
            this.timeout = timespan;
        }

        /// <summary>
        /// Get assicate data by thread safe way
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>The associate object</returns>
        private MemoryTokenData GetAssociateData(string token)
        {
            MemoryTokenData tokenData = null;
            lock (this.tokens)
            {
                if (this.tokens.ContainsKey(token))
                    tokenData = this.tokens[token];
            }

            return tokenData;
        }
    }
}
