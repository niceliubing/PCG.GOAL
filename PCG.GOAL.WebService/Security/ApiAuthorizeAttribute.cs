
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace PCG.GOAL.WebService.Security
{
   
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.ReasonPhrase = "Unauthorized Web Service Request";

            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}