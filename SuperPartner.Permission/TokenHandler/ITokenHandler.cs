using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.TokenHandler
{
    /// <summary>
    /// Author: Robert
    /// The interface is token handler 
    /// It include timeout, token, the object related token
    /// </summary>
    public interface ITokenHandler
    {
        /// <summary>
        /// The method is genarate token which associate with a object
        /// </summary>
        /// <param name="obj">the object which associate with the token</param>
        /// <returns>return a token</returns>
        string GeneToken(object obj);

        /// <summary>
        /// update token expired time
        /// basically, it will be invoke in Authorization Filter
        /// </summary>
        /// <param name="token">token</param>
        void RefreshToken(string token);

        /// <summary>
        /// Remove the token
        /// </summary>
        /// <param name="token">token</param>
        void RemoveToken(string token);

        /// <summary>
        /// Update the associate object by token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="obj">The associate object</param>
        void UpdateAssociateDataByToken(string token, object obj);

        /// <summary>
        /// Get the associate object by token
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="token">token</param>
        /// <returns>the assicate object will return if the token exist, the null will be return if the token didn't exist</returns>
        T GetAssociateObjectByToken<T>(string token);

        /// <summary>
        /// Get timeout of token
        /// the unit is second
        /// </summary>
        /// <returns>return the timeout</returns>
        int GetTimeout();

        /// <summary>
        /// Set the token timeout
        /// the unit is second
        /// </summary>
        /// <param name="timespan"></param>
        void SetTimeout(int timespan);

        
    }
}
