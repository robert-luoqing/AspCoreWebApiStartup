using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SuperPartner.Model.Common;
using SuperPartner.Model.Exception;
using SuperPartner.Utils.Loggers;
using System.IO;

namespace SuperPartner.Filters
{
    /// <summary>
    /// Author: Robert
    /// The class is used to intercept exception include business exception and system exception 
    /// golbal handle the exception and output to client. 
    /// It also log the exception to difference level. It can use analyze what issue occur in app.
    /// </summary>
    public class SpExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            var response = new WsResponse();
            var exception = context.Exception;
            if (exception is SpException)
            {
                // It is business error. Just log it.
                var spException = exception as SpException;
                response.Trans.ErrorCode = spException.ErrorCode;
                Logger.Error(exception);
            }
            else
            {
                // It is unknown exception, It must resolve by developer is these kind of error occur
                response.Trans.ErrorCode = "9999";
                Logger.Fatal(exception);

                // Log as detail as possible
                var request = context.HttpContext.Request;
                Logger.Fatal("Request url: " + request.Path);
                Logger.Fatal("Request Query String: " + request.QueryString.ToString());
                request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(request.Body))
                {
                    var bodyContent = reader.ReadToEnd();
                    Logger.Fatal("Request body: " + bodyContent);
                }
            }

            response.Trans.ErrorMsg = exception.Message;
            var result = new JsonResult(response);
            result.StatusCode = 404;
            context.Result = result;
        }
    }
}
