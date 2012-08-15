using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using ReSTCore.ActionResults;
using ReSTCore.Attributes;
using ReSTCore.DTO;
using ReSTCore.Mapping;
using ReSTCore.Models;
using ReSTCore.ResponseFormatting;
using ReSTCore.Routing;
using ReSTCore.Util;
using JsonResult = ReSTCore.ActionResults.JsonResult;

namespace ReSTCore.Controllers
{
    /// <typeparam name="TId">The type of the ID of this object controlled by the controller (e.g. 0 int or string)</typeparam>
    /// <typeparam name="TEntity">The type of the object controlled by this controller</typeparam>
    public class BaseController<TId, TEntity> : Controller
        where TEntity : RestEntity<TId>
    {
        /// <summary>
        /// Lists objects
        /// Method: GET
        /// Uri:    /[controller]
        /// </summary>
        [Help(Ignore = true)]
        public virtual ActionResult Index()
        {
            SetMethodNotAllowed();
            return null;
        }

        /// <summary>
        /// Returns one object
        /// Method: GET
        /// Uri:    /[controller]/[id]
        /// </summary>
        [Help(Ignore = true)]
        public virtual ActionResult Show(TId id)
        {
            SetMethodNotAllowed();
            return null;
        }

        /// <summary>
        /// Returns one object
        /// Method: GET
        /// Uri:    /[controller]/[id]/[property]
        /// </summary>
        [Help(Ignore = true)]
        public virtual ActionResult ShowProperty(TId id, string property)
        {
            SetMethodNotAllowed();
            return null;
        }

        /// <summary>
        /// Creates a new object
        /// Method: POST
        /// Uri:    /[controller]
        /// </summary>
        [Help(Ignore = true)]
        public virtual ActionResult Create(TEntity obj)
        {
            SetMethodNotAllowed();
            return null;
        }

        /// <summary>
        /// Edits an existing object
        /// Method: PUT
        /// Uri:    /[controller]/[id]
        /// </summary>
        [Help(Ignore = true)]
        public virtual ActionResult Update(TId id, TEntity obj)
        {
            SetMethodNotAllowed();
            return null;
        }

        /// <summary>
        /// Edits an existing object
        /// Method: PUT
        /// Uri:    /[controller]/[id]/[property]
        /// </summary>
        [Help(Ignore = true)]
        public virtual ActionResult UpdateProperty(TId id, string property, string value)
        {
            SetMethodNotAllowed();
            return null;
        }

        /// <summary>
        /// Deletes an existing object
        /// Method: DELETE
        /// Uri:    /[controller]/[id]
        /// </summary>
        [Help(Ignore = true)]
        public virtual ActionResult Delete(TId id)
        {
            SetMethodNotAllowed();
            return null;
        }

        /// <summary>
        /// Method: GET
        /// Uri:    /[controller]/help
        /// </summary>
        /// <returns></returns>
        [Help("Shows help information for the current controller")]
        public ActionResult Help()
        {
            var model = new HelpModel(this);
            return View("~/Views/RestCore/Help.cshtml", model);
        }

        protected bool ValidateCreate(RestEntity<TId> entity)
        {
            if (entity == null)
            {
                SetResponseStatus(HttpStatusCode.BadRequest, "Object to create must be provided");
                return false;
            }

            return ValidateEntity(entity);
        }

        protected bool ValidateUpdate(TId id, RestEntity<TId> entity)
        {
            if (entity == null)
            {
                SetResponseStatus(HttpStatusCode.BadRequest, "Object to update must be provided");
                return false;
            }

            if (id == null)
            {
                SetResponseStatus(HttpStatusCode.BadRequest, "ID must be provided on update");
                return false;
            }

            if (id is string && string.IsNullOrWhiteSpace(id.ToString()))
            {
                SetResponseStatus(HttpStatusCode.BadRequest, "ID must not be empty");
                return false;
            }

            return ValidateEntity(entity);
        }

        protected object GetProperty(object entity, string property)
        {
            Type type = entity.GetType();
            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.Name.Equals(property, StringComparison.OrdinalIgnoreCase))
                    continue;
                object value = propertyInfo.GetValue(entity, null);
                return new PropertyResult<object> {{propertyInfo.Name, value}};
            }
            return null;
        }

        protected bool SetProperty(object entity, string property, string value)
        {
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
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    int newInt;
                    if (int.TryParse(value, out newInt))
                        propertyInfo.SetValue(entity, newInt, null);
                    else
                        return false;
                }
                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    Guid newGuid;
                    if (Guid.TryParse(value, out newGuid))
                        propertyInfo.SetValue(entity, newGuid, null);
                    else
                        return false;
                }
                else
                    return false;
                break;
            }
            return propertyFound;
        }

        private bool ValidateEntity(object entity)
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
                SetResponseStatus(HttpStatusCode.BadRequest, message.ToString());
                return false;
            }
            return true;
        }

        protected ActionResult HandleGetResult<T>(T entity, bool map = false) where T : class
        {
            Result<T> result;
            if (entity == null)
                result = new Result<T> {HttpStatusCode = HttpStatusCode.NotFound, ResultType = ResultType.ClientError};
            else
                result = new Result<T> {ResultType = ResultType.Success, Entity = entity};
            return HandleResult<T>(RestfulAction.Show, result, map);
        }

        protected ActionResult HandleResult<T>(RestfulAction action, Result<T> result, bool map = false) where T : class
        {
            if (result.ResultType == ResultType.Success)
                if(map)
                    return MapSuccessResult(action, result.Entity);
                else
                    return SuccessResult(action, result.Entity);

            HttpStatusCode httpStatusCode;
            if (result.HttpStatusCode == null)
                httpStatusCode = result.ResultType == ResultType.ClientError
                    ? action == RestfulAction.Show ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest
                        : HttpStatusCode.InternalServerError;
            else
                httpStatusCode = result.HttpStatusCode.Value;
            return ErrorResult(httpStatusCode, result.ErrorCode, result.ErrorMessage);
        }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="action">The Restful action type (create, show, update, delete, index)</param>
        /// <param name="dto">The dto to return</param>
        protected ActionResult SuccessResult<T>(RestfulAction action, T dto) where T : class
        {
            string uri = string.Empty;
            if (dto != null && dto.GetType().IsInstanceOfType(typeof(RestEntity<TId>)))
                uri = (dto as RestEntity<TId>).Uri;
            
            switch(action)
            {
                case RestfulAction.Index:
                case RestfulAction.Show:
                    SetResponseStatus(HttpStatusCode.OK);
                    return DynamicResult(dto);
                case RestfulAction.Create:
                    SetResponseStatus(HttpStatusCode.Created);
                    Response.AddHeader("Content-Location", uri);
                    return null;
                case RestfulAction.Update:
                    SetResponseStatus(HttpStatusCode.Accepted);
                    Response.AddHeader("Content-Location", uri);
                    return null;
                case RestfulAction.Delete:
                    SetResponseStatus(HttpStatusCode.OK);
                    return null;
                default:
                    SetResponseStatus(HttpStatusCode.BadRequest, "Unknown action");
                    return null;
            }
        }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="action">The Restful action type (create, show, update, delete, index)</param>
        /// <param name="sources">The source objects</param>
        private ActionResult MapSuccessResult(RestfulAction action, params object[] sources)
        {
            TEntity dto = null;
            if (sources.Any())
                dto = EntityMapper.Map<TEntity>(sources);
            return SuccessResult(action, dto);
        }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="action">The Restful action type (create, show, update, delete, index)</param>
        /// <param name="sources">The source objects</param>
        private ActionResult MapSuccessResult(RestfulAction action, IEnumerable<object> sources)
        {
            var dtos = Mapper.Map<List<TEntity>>(sources);
            return SuccessResult(action, dtos);
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
        /// <returns></returns>
        private ActionResult DynamicResult(object result)
        {
            ResponseFormatType bodyFormat = new ResponseFormatDecider(ResponseMappingSettings.Settings).Decide(HttpContext.Request.AcceptTypes, HttpContext.Request.QueryString);
            switch (bodyFormat)
            {
                case ResponseFormatType.Xml:
                    return new XmlResult(result);
                case ResponseFormatType.Json:
                    return new JsonResult(result);
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
        private void SetMethodNotAllowed()
        {
            string msg = string.Format("Action {0} is not an acceptable action for this resource.", ControllerContext.RouteData.GetRequiredString("action"));
            SetResponseStatus(HttpStatusCode.MethodNotAllowed, msg);
        }

        /// <summary>
        /// This method will accept a status code and message and modify the response according to the RFC2616 specification
        /// </summary>
        private void SetResponseStatus(HttpStatusCode httpStatusCode, string errorMessage = null, int? errorCode = null)
        {
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
                Response.AddHeader("X-ServiceErrorMessage", errorMessage);

            if (errorCode != null)
                Response.AddHeader("X-ServiceErrorCode", errorCode.ToString());
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            SetResponseStatus(HttpStatusCode.InternalServerError, filterContext.Exception.Message);
            filterContext.Result = null;
        }
    }
}
