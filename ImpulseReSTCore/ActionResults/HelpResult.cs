using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor;
using System.Web.Routing;
using ImpulseReSTCore.Models;

namespace ImpulseReSTCore.ActionResults
{
    public class HelpResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "text/html";

            //var html = new StringBuilder("<html><body>");
            //html.Append("<table>");
            //foreach (Route route in RouteTable.Routes)
            //{
            //    html.Append("<tr>");
            //    html.Append("<td>" + route.Url + "</td>");
            //    html.Append("</tr>");
            //}
            //html.Append("</table>");


            //html.Append("</body></html>");

            //response.Write(html.ToString());

            var viewData = new ViewDataDictionary();
            var tempData = new TempDataDictionary();

            string result = "";
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(context, "~/bin/RestViews/Help.cshtml", null);
                ViewContext viewContext = new ViewContext(context, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);

                result = sw.GetStringBuilder().ToString();
            }

            response.Write(result);
        }
    }
}
