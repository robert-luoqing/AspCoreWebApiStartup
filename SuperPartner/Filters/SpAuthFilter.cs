using Microsoft.AspNetCore.Mvc.Filters;
using SuperPartner.Biz.Common;
using SuperPartner.Permission.TokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperPartner.Filters
{
    public class SpAuthFilter : IAuthorizationFilter
    {
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenHandler = context.HttpContext.RequestServices.GetService(typeof(ITokenHandler)) as ITokenHandler;
            var spContext = context.HttpContext.RequestServices.GetService(typeof(SpContext)) as SpContext;
            spContext.TokenHandler = tokenHandler;
            spContext.Token = Guid.NewGuid().ToString();
            
        }
    }
}
