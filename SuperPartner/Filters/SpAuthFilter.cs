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
            var spContext = context.HttpContext.RequestServices.GetService(typeof(BizContext)) as BizContext;
            spContext.TokenHandler = tokenHandler;

            var token = context.HttpContext.Request.Headers["token"];
            spContext.Token = token;
            if(!string.IsNullOrEmpty(token))
            {
                spContext.LoginUser = tokenHandler.GetAssociateObjectByToken<LoginUser>(token);
            }
        }
    }
}
