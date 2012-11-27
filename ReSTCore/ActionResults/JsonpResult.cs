using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ReSTCore.DTO;

namespace ReSTCore.ActionResults
{
    public class JsonpResult : System.Web.Mvc.JsonResult
    {
        public JsonpResult(object data)
        {
            Data = data;
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/javascript";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data == null)
                return;

            string callback = context.HttpContext.Request.QueryString["callback"];
            if (string.IsNullOrWhiteSpace(callback))
                callback = "callback";

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            var serializedObject = JsonConvert.SerializeObject(Data, Formatting.None, serializerSettings);
            response.Write(callback + "(" + serializedObject + ");");
        }
    }
}
