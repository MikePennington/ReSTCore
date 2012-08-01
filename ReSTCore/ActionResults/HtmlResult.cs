using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace ReSTCore.ActionResults
{

    class HtmlResult : ActionResult
    {
        private readonly object _objectToSerialize;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlResult"/> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to Html.</param>
        public HtmlResult(object objectToSerialize)
        {
            _objectToSerialize = objectToSerialize;
        }

        /// <summary>
        /// Gets the object to be serialized to Html.
        /// </summary>
        public object ObjectToSerialize
        {
            get { return _objectToSerialize; }
        }


        /// <summary>
        /// Serialises the object that was passed into the constructor to HTML and writes the corresponding HTML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "text/html";

            var viewData = new ViewDataDictionary();
            var tempData = new TempDataDictionary();

            string result = "";
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(context, "~/bin/RestViews/Html.cshtml", null);
                ViewContext viewContext = new ViewContext(context, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);

                result = sw.GetStringBuilder().ToString();
            }

            response.Write(result);
        }

    }
}
