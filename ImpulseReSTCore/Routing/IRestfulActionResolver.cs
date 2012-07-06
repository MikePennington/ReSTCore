using System.Web.Routing;

namespace ImpulseReSTCore.Routing
{
    public interface IRestfulActionResolver
    {
        RestfulAction ResolveAction(RequestContext context);
    }
}