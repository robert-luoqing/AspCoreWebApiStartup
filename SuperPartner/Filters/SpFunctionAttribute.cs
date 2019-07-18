using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SuperPartner.Biz.Common;
using SuperPartner.Model.Common;
using SuperPartner.Permission.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperPartner.Filters
{
    /// <summary>
    /// It is attribute which use to check whether the user can access the functions.
    /// </summary>
    public class SpFunctionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// If one of functions has passed permission check, then the authorizion have passed if passedByOneOf is true.
        /// else all of functions must be passed.
        /// </summary>
        private bool passedByOneOf = true;
        /// <summary>
        /// The functions which need to checked
        /// </summary>
        private string[] functions;

        public SpFunctionAttribute(params string[] functions)
        {
            this.functions = functions;
        }

        public SpFunctionAttribute(bool passedByOneOf, params string[] functions)
        {
            this.passedByOneOf = passedByOneOf;
            this.functions = functions;
        }

        /// <summary>
        /// Check permission according to the functions
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var bizContext = context.HttpContext.RequestServices.GetService(typeof(BizContext)) as BizContext;
            var authorizationHandler = context.HttpContext.RequestServices.GetService(typeof(IAuthorizationHandler)) as IAuthorizationHandler;

            // if no login, pass 401 error
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
                // Check does the user can access these funcs
                var funcCount = this.functions.Length;
                int passedCount = 0, failedCount = 0;
                foreach (var funcCode in this.functions)
                {
                    var hasPermission = authorizationHandler.CheckFunction(bizContext.LoginUser.UserId.ToString(), funcCode);
                    if (hasPermission)
                        passedCount++;
                    else
                        failedCount++;
                }

                // If there are no func need check 
                // or all successful, then passed
                // or one of them passed and passedByOneOf is true, then passed
                if (funcCount == 0
                    || failedCount == 0
                    || (this.passedByOneOf && passedCount > 0))
                    base.OnActionExecuting(context);
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

