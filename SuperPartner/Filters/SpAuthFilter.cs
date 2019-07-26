using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SuperPartner.Biz.Common;
using SuperPartner.Model.Common;
using SuperPartner.Model.Organization.User;
using SuperPartner.Permission.Authorization;
using SuperPartner.Permission.TokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperPartner.Filters
{
    /// <summary>
    /// The authorization filter
    /// It used to verify login or non-login API, and initial token, token's data
    /// </summary>
    public class SpAuthFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenHandler = context.HttpContext.RequestServices.GetService(typeof(ITokenHandler)) as ITokenHandler;
            var bizContext = context.HttpContext.RequestServices.GetService(typeof(BizContext)) as BizContext;
            bizContext.TokenHandler = tokenHandler;

            var token = context.HttpContext.Request.Headers["token"];
            bizContext.Token = token;
            if (!string.IsNullOrEmpty(token))
            {
                bizContext.LoginUser = tokenHandler.GetAssociateObjectByToken<LoginUser>(token);
            }

            if (bizContext.Configuration.NeedCheckPermissionFromUrl)
            {
                var url = context.HttpContext.Request.Path.Value;
                var authorizationHandler = context.HttpContext.RequestServices.GetService(typeof(IAuthorizationHandler)) as IAuthorizationHandler;
                var isPassed = authorizationHandler.CheckUrl(bizContext.LoginUser.UserId.ToString(),
                    url,
                    bizContext.Configuration.IgnoreCheckPermissionUrls,
                    bizContext.Configuration.IgnoreCheckPermissionUrlsWhenLogined);
                if (isPassed) return; // passed check permission

                // failed to check url according permission
                if (bizContext.LoginUser == null)
                {
                    var response = new WsResponse();
                    response.Trans.ErrorCode = "401";
                    response.Trans.ErrorMsg = "Not login";
                    var result = new JsonResult(response);
                    result.StatusCode = 401;
                    context.Result = result;
                }
                else
                {
                    var response = new WsResponse();
                    response.Trans.ErrorCode = "403";
                    response.Trans.ErrorMsg = "Authorization Failed";
                    var result = new JsonResult(response);
                    result.StatusCode = 403;
                    context.Result = result;
                }
            }
        }
    }
}
