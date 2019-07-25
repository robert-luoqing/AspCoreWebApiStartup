using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.Authorization
{
    /// <summary>
    /// The interface is used to save the authorization settings.
    /// Like save the functions which assign to user
    /// It only include assign function to user.
    /// </summary>
    public interface IAuthorizationStorageProvider
    {
        /// <summary>
        /// Create a function in storage
        /// Notice FuncCode must uniquer
        /// </summary>
        /// <param name="func">Function object</param>
        void CreateFunc(PermFunc func);
        /// <summary>
        /// Remove function from storage
        /// </summary>
        /// <param name="funcCode">Funcaction Code</param>
        void RemoveFunc(string funcCode);
        /// <summary>
        /// Get function objects by condition
        /// </summary>
        /// <param name="includeDesc">Does the return object inlucdes description</param>
        /// <param name="includeExtendProperties">Does the return object includes extend properties</param>
        /// <returns></returns>
        List<PermFunc> GetFuncs(bool includeDesc, bool includeExtendProperties);

        /// <summary>
        /// Assign functions to user.
        /// Notice: The operation will be do below step
        /// 1. Remove all funcions from the user
        /// 2. Batch add funcCodes to the user.
        /// It means the user only has funcCodes
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="funcCodes">function codes which will assign to the user</param>
        void AssignWholeFuncsToUser(string userId, List<FuncAssignation> funcCodes);
        /// <summary>
        /// Add func to user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="funcCode">The func code, like "User Operation" etc</param>
        /// <param name="level">The level, sometimes, the requirement need implement the functions like we need assign user to access whole department user</param>
        void AddFuncToUser(string userId, string funcCode, AccessLevel level);
        /// <summary>
        /// Remove a func from User
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="funcCode">Function code which will be removed from user</param>
        void RemoveFuncFromUser(string userId, string funcCode);
        /// <summary>
        /// Get all functions by user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>the functions which assign to the user</returns>
        List<FuncAssignation> GetFuncsByUser(string userId);
    }
}
