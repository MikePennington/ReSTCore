using System;
using System.Web.Routing;

namespace ImpulseReSTCore.Routing
{
    public class RestfulActionResolver : IRestfulActionResolver
    {
        public RestfulAction ResolveAction(RequestContext context)
        {
            if (context.HttpContext.Request == null)
                throw new NullReferenceException("Request in RequestContext.HttpContext cannot be null.");
            if (string.IsNullOrEmpty(context.HttpContext.Request.HttpMethod) ||
                !string.Equals(context.HttpContext.Request.HttpMethod.ToUpperInvariant(), "POST", StringComparison.Ordinal))
                return RestfulAction.None;
            return ResolvePostAction(context);
        }

        private static RestfulAction ResolvePostAction(RequestContext context)
        {
            if (context.HttpContext.Request.Form == null)
                return RestfulAction.None;
            string str = context.HttpContext.Request.Form["_method"];
            if (string.IsNullOrEmpty(str))
                return RestfulAction.None;
            string b = str.Trim().ToUpperInvariant();
            if (string.Equals("PUT", b))
                return RestfulAction.Update;
            if (string.Equals("DELETE", b, StringComparison.Ordinal))
                return RestfulAction.Destroy;
            return RestfulAction.None;
        }
    }
}