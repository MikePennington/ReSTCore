using System.Web.Routing;

namespace ReSTCore.Routing
{
    public interface IRestfulActionResolver
    {
        RestfulAction ResolveAction(RequestContext context);
    }
}