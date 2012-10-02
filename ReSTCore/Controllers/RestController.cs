using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using ReSTCore.ActionResults;
using ReSTCore.Attributes;
using ReSTCore.Models;
using ReSTCore.ResponseFormatting;
using ReSTCore.Util;

namespace ReSTCore.Controllers
{
    public class RestController : Controller
    {
        /// <summary>
        /// Method: GET
        /// Uri:    /[controller]/help
        /// </summary>
        /// <returns></returns>
        [Help(Ignore = true)]
        public ActionResult Help()
        {
            var model = new HelpModel(this);
            return View("~/Views/RestCore/Help.cshtml", model);
        }

        /// <summary>
        /// Can be used as a helper method for the ShowProperty action
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        protected object GetProperty(object entity, string property)
        {
            if (entity == null || string.IsNullOrWhiteSpace(property))
                return null;

            Type type = entity.GetType();
            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.Name.Equals(property, StringComparison.OrdinalIgnoreCase))
                    continue;
                object value = propertyInfo.GetValue(entity, null);
                return new PropertyResult<object> { { propertyInfo.Name, value } };
            }
            return null;
        }

        /// <summary>
        /// Can be used as a helper method for the UpdateProperty action
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool SetProperty(object entity, string property, string value)
        {
            if (entity == null || string.IsNullOrWhiteSpace(property))
                return false;

            Type type = entity.GetType();
            bool propertyFound = false;
            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.Name.Equals(property, StringComparison.OrdinalIgnoreCase))
                    continue;
                propertyFound = true;
                if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(entity, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(short))
                {
                    short newPropertyValue;
                    if (short.TryParse(value, out newPropertyValue))
                        propertyInfo.SetValue(entity, newPropertyValue, null);
                    else
                        return false;
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    int newPropertyValue;
                    if (int.TryParse(value, out newPropertyValue))
                        propertyInfo.SetValue(entity, newPropertyValue, null);
                    else
                        return false;
                }
                else if (propertyInfo.PropertyType == typeof(long))
                {
                    long newPropertyValue;
                    if (long.TryParse(value, out newPropertyValue))
                        propertyInfo.SetValue(entity, newPropertyValue, null);
                    else
                        return false;
                }
                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    Guid newPropertyValue;
                    if (Guid.TryParse(value, out newPropertyValue))
                        propertyInfo.SetValue(entity, newPropertyValue, null);
                    else
                        return false;
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    DateTime newPropertyValue;
                    if (DateTime.TryParse(value, out newPropertyValue))
                        propertyInfo.SetValue(entity, newPropertyValue, null);
                    else
                        return false;
                }
                else
                    return false;
                break;
            }
            return propertyFound;
        }

        protected bool ValidateEntity(object entity)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(entity, null, null);
            bool isValid = Validator.TryValidateObject(entity, context, results, true);
            if (!isValid)
            {
                var message = new StringBuilder();
                message.Append(results.Count == 0
                                   ? "Invalid input"
                                   : string.Join(", ", results.Select(x => x.ErrorMessage).ToArray()));
                SetResponseStatus(HttpStatusCode.BadRequest, message.ToString(), RestCore.Configuration.InvalidUserIntputErrorCode);
                return false;
            }
            return true;
        }

                /// <summary>
        /// Used to return an error state
        /// </summary>
        /// <param name="httpStatusCode">The Http status code</param>
        /// <param name="errorCode">The service-specific  error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns></returns>
        protected ActionResult ErrorResult(HttpStatusCode httpStatusCode, int? errorCode = null, string errorMessage = null)
        {
            SetResponseStatus(httpStatusCode, errorMessage, errorCode);
            return DynamicResult(null);
        }

        /// <summary>
        /// This should only be called directly from the controller if you are not returning the base entity.
        /// </summary>
        /// <param name="result">The result to return</param>
        /// <param name="acceptTypes">Accept types from request. Will try to pull this from the request if it is null.</param>
        /// <param name="querystring">Querystring from request. Will try to pull this from the request if it is null.</param>
        /// <returns></returns>
        protected ActionResult DynamicResult(object result, string[] acceptTypes = null, NameValueCollection querystring = null)
        {
            if (acceptTypes == null && HttpContext != null && HttpContext.Request != null)
                acceptTypes = HttpContext.Request.AcceptTypes;
            if (querystring == null && HttpContext != null && HttpContext.Request != null)
                querystring = HttpContext.Request.QueryString;
            ResponseFormatType bodyFormat = new ResponseFormatDecider(ResponseMappingSettings.Settings)
                .Decide(acceptTypes, querystring);
            switch (bodyFormat)
            {
                case ResponseFormatType.Xml:
                    return new XmlResult(result);
                case ResponseFormatType.Json:
                    return new ActionResults.JsonResult(result);
                case ResponseFormatType.Jsonp:
                    return new JsonpResult(result);
                case ResponseFormatType.Html:
                    var model = new HtmlModel(result);
                    return View("~/Views/RestCore/Html.cshtml",model);
                default:
                    throw new Exception("Unknown response format type");
            }
        }

        protected bool GetParameterAsBool(string paramName, bool defaultValue = false)
        {
            string paramValue = Request.Params[paramName];
            bool returnValue;
            if(!bool.TryParse(paramValue, out returnValue))
                returnValue = defaultValue;
            return returnValue;
        }

        protected int GetParameterAsInt(string paramName, int defaultValue = 0)
        {
            string paramValue = Request.Params[paramName];
            int returnValue;
            if (!int.TryParse(paramValue, out returnValue))
                returnValue = defaultValue;
            return returnValue;
        }

        protected string GetParameterAsString(string paramName, string defaultValue = null)
        {
            string paramValue = Request.Params[paramName] ?? defaultValue;
            return paramValue;
        }

        /// <summary>
        /// This method will set the Method Not Allowed status code - Because an action is not available in the given context
        /// 10.4.6 405 Method Not Allowed
        /// </summary>
        protected void SetMethodNotAllowed()
        {
            string msg = string.Format("Action {0} is not an acceptable action for this resource.", ControllerContext.RouteData.GetRequiredString("action"));
            SetResponseStatus(HttpStatusCode.MethodNotAllowed, msg);
        }

        /// <summary>
        /// This method will accept a status code and message and modify the response according to the RFC2616 specification
        /// </summary>
        protected void SetResponseStatus(HttpStatusCode httpStatusCode, string errorMessage = null, int? errorCode = null)
        {
            if (Response == null)
                return;

            Response.StatusCode = (int)httpStatusCode;
            Response.TrySkipIisCustomErrors = true;

            if (httpStatusCode == HttpStatusCode.NotModified
                || httpStatusCode == HttpStatusCode.NotFound
                || httpStatusCode == HttpStatusCode.MethodNotAllowed
                || httpStatusCode == HttpStatusCode.UnsupportedMediaType
                || httpStatusCode == HttpStatusCode.MovedPermanently)
            {
                Response.SuppressContent = true;
            }

            if(!string.IsNullOrWhiteSpace(errorMessage))
                Response.AddHeader(Constants.Headers.ErrorMessage, errorMessage);

            if (errorCode != null)
                Response.AddHeader(Constants.Headers.ErrorCode, errorCode.ToString());
        }

        protected override void OnException(ExceptionContext exceptionContext)
        {
            SetResponseStatus(HttpStatusCode.InternalServerError,
                              RestCore.Configuration.HideRealException
                                  ? RestCore.Configuration.DefaultExceptionText
                                  : exceptionContext.Exception.Message,
                              RestCore.Configuration.UnknownServerErrorCode);
            exceptionContext.Result = null;
            exceptionContext.ExceptionHandled = true;
        }
    }
}
