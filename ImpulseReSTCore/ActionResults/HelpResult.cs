using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ImpulseReSTCore.ActionResults
{
    public class HelpResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "text/html";

            var html = new StringBuilder("<html><body>");
            html.Append("<table>");
            foreach (Route route in RouteTable.Routes)
            {
                html.Append("<tr>");
                html.Append("<td>" + route.Url + "</td>");
                html.Append("</tr>");
            }
            html.Append("</table>");


            html.Append("</body></html>");

            response.Write(html.ToString());
        }
    }
}
