using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SuperPartner.Biz.Common;
using SuperPartner.Model.Organization.User;
using SuperPartner.Permission.TokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
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

            //if (bizContext.Configuration.NeedCheckPermissionFromUrl)
            //{
                
            //}
        }
    }
}
