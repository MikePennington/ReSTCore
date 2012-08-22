using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ReSTCore.DTO;

namespace ReSTCore.ActionResults
{
    public class JsonResult : System.Web.Mvc.JsonResult
    {
        public JsonResult(object data)
        {
            Data = data;
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data.GetType() == typeof(StringDTO))
                Data = new {value = ((StringDTO) Data).Value};

            if (Data != null)
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.Converters.Add(new IsoDateTimeConverter());
                var serializedObject = JsonConvert.SerializeObject(Data, Formatting.None, serializerSettings);
                response.Write(serializedObject);
            }
        }
    }
}
