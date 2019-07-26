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
        /// <param name="ignoreUrls">Those url will not to check in CheckUrl method. It means the checked url in those urls will reutnr true</param>
        /// <param name="ignoreUrlsForLoginUser">Those url will not to check in CheckUrl method when it is login user. It means the checked url in those urls for login users will reutnr true</param>
        /// <returns>return true if user can access the url, otherwise will return false</returns>
        bool CheckUrl(string userId, string url, List<string> ignoreUrls, List<string> ignoreUrlsForLoginUser);

        /// <summary>
        /// The does user have permission have the funcCode permission
        /// </summary>
        /// <param name="userId">The login user id</param>
        /// <param name="funcCode">The funcation code which defined in system</param>
        /// <returns></returns>
        bool CheckFunction(string userId, string funcCode);

        /// <summary>
        /// Does the permission will be cache in memory to make better performance
        /// </summary>
        bool UseCache { get; set; }

        /// <summary>
        /// Clear cache if UseCache is true
        /// </summary>
        void ClearCache();
    }
}
