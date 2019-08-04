using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace SuperPartner.Permission.Authorization
{
    public class AuthorizationHandler : IAuthorizationHandler
    {
        /// <summary>
        /// It is memory cache which store the user's func.
        /// The key
        /// </summary>
        private Dictionary<string, List<FuncAssignation>> permissionsCache = new Dictionary<string, List<FuncAssignation>>();

        /// <summary>
        /// It is func cache which will gain the performance.
        /// But only available when UseCache is true
        /// </summary>
        private Dictionary<string, PermFunc> funcsCache = null;

        /// <summary>
        /// Storage provider which provide fetch data from database or storage files
        /// </summary>
        private IAuthorizationStorageProvider storageProvider;

        public AuthorizationHandler(IAuthorizationStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider;
        }

        public bool UseCache { get; set; }

        /// <summary>
        /// Check permission
        /// </summary>
        /// <remarks>
        /// If the user's permission is in memory, then check it in memory
        /// else load it from storage
        /// If there is function which matched the funcCode and it is not equal DenyAccessData, It will pass to check
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="funcCode"></param>
        /// <returns></returns>
        public bool CheckFunction(string userId, string funcCode)
        {
            var userFuncs = GetFuncsByUser(userId);

            // Check the permission
            if (userFuncs == null)
            {
                return false;
            }
            else
            {
                var matchedFunc = userFuncs.Where(o => o.FuncCode == funcCode).FirstOrDefault();
                if (matchedFunc == null)
                {
                    return false;
                }
                else
                {
                    if (matchedFunc.AccessLevel == AccessLevel.DenyAccessData)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// According to the url to check if the user have permission
        /// </summary>
        /// <param name="userId">User which need to check</param>
        /// <param name="url">The url, notice: the url does NOT case sensitive</param>
        /// <param name="ignoreUrls">Those url will not to check in CheckUrl method. It means the checked url in those urls will reutnr true</param>
        /// <param name="ignoreUrlsForLoginUser">Those url will not to check in CheckUrl method when it is login user. It means the checked url in those urls for login users will reutnr true</param>
        /// <returns>return true if the user have url permission</returns>
        public bool CheckUrl(string userId, string url, List<string> ignoreUrls, List<string> ignoreUrlsForLoginUser)
        {
            // Check ignore url
            bool isIngore = CheckIfMatched(ignoreUrls, url);
            if (isIngore) return true;

            // If no login, then return false;
            if (string.IsNullOrWhiteSpace(userId)) return false;

            // Check ignore url for login user
            isIngore = CheckIfMatched(ignoreUrlsForLoginUser, url);
            if (isIngore) return true;

            // Get user's function
            var userFuncs = GetFuncsByUser(userId);

            // Check the permission
            if (userFuncs == null)
            {
                return false;
            }
            else
            {
                var funcs = GetFunctionsByFuncCode();
                foreach (var func in userFuncs)
                {
                    if (func.AccessLevel != AccessLevel.DenyAccessData && funcs.ContainsKey(func.FuncCode))
                    {
                        var funcObj = funcs[func.FuncCode];
                        if (!string.IsNullOrWhiteSpace(funcObj.AssociateUrls))
                        {
                            var funcUrls = funcObj.AssociateUrls.Split("\n".ToCharArray()).ToList();
                            var matched = CheckIfMatched(funcUrls, url);
                            if (matched) return true;
                        }
                    }
                }

                return false;
            }
        }

        public void ClearCache()
        {
            funcsCache = null;
            permissionsCache.Clear();
        }

        /// <summary>
        /// Get user's functions
        /// </summary>
        /// <remarks>
        /// If the user's permission is in memory, then check it in memory
        /// else load it from storage
        /// </remarks>
        /// <param name="userId">User Id</param>
        /// <returns>The user's functions</returns>
        private List<FuncAssignation> GetFuncsByUser(string userId)
        {
            List<FuncAssignation> userFuncs = null;
            // Get user's function from momery cache if UseCache is enabled
            if (this.UseCache == true)
            {
                if (permissionsCache.ContainsKey(userId))
                {
                    userFuncs = permissionsCache[userId];
                }
            }

            // Load user's function from storage provider
            if (userFuncs == null)
            {
                userFuncs = storageProvider.GetFuncsByUser(userId);
                if (this.UseCache == true && userFuncs != null)
                {
                    permissionsCache.Add(userId, userFuncs);
                }
            }

            return userFuncs;
        }

        /// <summary>
        /// Get funcs object
        /// </summary>
        /// <remarks>
        /// If the funcs is in memory, then get from memory when UseCache is true
        /// else load it from storage
        /// Notice, Convert to dictionary is using to gain performance
        /// </remarks>
        /// <returns>The functions</returns>
        private Dictionary<string, PermFunc> GetFunctionsByFuncCode()
        {
            if (this.UseCache && funcsCache != null)
            {
                return funcsCache;
            }

            var results = new Dictionary<string, PermFunc>();
            var funcList = this.storageProvider.GetFuncs(false, false);
            if (funcList != null)
            {
                foreach (var item in funcList)
                {
                    results.Add(item.FuncCode, item);
                }

                if (this.UseCache)
                {
                    funcsCache = results;
                }
            }

            return results;
        }

        private bool CheckIfMatched(List<string> urlPatterns, string url)
        {
            if (urlPatterns == null) urlPatterns = new List<string>();
            var isIngore = false;
            foreach (var ignoreUrl in urlPatterns)
            {
                var matched = Regex.IsMatch(url, ignoreUrl.Trim(), RegexOptions.IgnoreCase);
                if (matched)
                {
                    isIngore = true;
                    break;
                }
            }

            return isIngore;
        }
    }
}
