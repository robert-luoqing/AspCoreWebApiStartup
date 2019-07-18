using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.Authorization
{
    /// <summary>
    /// The interface is used to handle authorization
    /// </summary>
    public interface IAuthorizationHandler
    {
        /// <summary>
        /// Check does the url can be access by the user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="url">The url</param>
        /// <returns>return true if user can access the url, otherwise will return false</returns>
        bool CheckUrl(string userId, string url);

        /// <summary>
        /// The does user have permission have the funcCode permission
        /// </summary>
        /// <param name="userId">The login user id</param>
        /// <param name="funcCode">The funcation code which defined in system</param>
        /// <returns></returns>
        bool CheckFunction(string userId, string funcCode);
    }
}
