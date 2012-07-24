using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using ImpulseReSTCore.ActionResults;
using ImpulseReSTCore.Attributes;
using ImpulseReSTCore.DTO;
using ImpulseReSTCore.Mapping;
using ImpulseReSTCore.Models;
using ImpulseReSTCore.ResponseFormatting;
using ImpulseReSTCore.Routing;

namespace ImpulseReSTCore
{
    /// <typeparam name="TId">The type of the ID of this object controlled by the controller (e.g. 0 int or string)</typeparam>
    /// <typeparam name="TEntity">The type of the object controlled by this controller</typeparam>
    public class BaseController<TId, TEntity> : Controller
        where TEntity : RestEntity<TId>
    {
        protected ResponseMappingSettings Settings { get; set; }

        public BaseController()
        {
            Settings = ResponseMappingSettings.DefaultSettings;
        }

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
        /// Deletes an existing object
        /// Method: DELETE
        /// Uri:    /[controller]/[id]
        /// </summary>
        [Help(Ignore = true)]
        public virtual ActionResult Destroy(TId id)
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
            return View("~/Views/Rest/Help.cshtml", model);
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

        private bool ValidateEntity(RestEntity<TId> entity)
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

        protected ActionResult HandleResult(RestfulAction action, Result<TEntity> result)
        {
            if (result.ResultType == ResultType.Success)
                return MapSuccessResult(action, result.Entity);

            HttpStatusCode httpStatusCode;
            if (result.HttpStatusCode == null)
                httpStatusCode = result.ResultType == ResultType.ClientError
                                     ? HttpStatusCode.BadRequest
                                     : HttpStatusCode.InternalServerError;
            else
                httpStatusCode = result.HttpStatusCode.Value;
            return ErrorResult(httpStatusCode, result.ErrorCode, result.ErrorMessage);
        }

        protected ActionResult HandleResult(RestfulAction action, Result<List<TEntity>> result)
        {
            if (result.ResultType == ResultType.Success)
                return MapSuccessResult(action, result.Entity);

            HttpStatusCode httpStatusCode;
            if (result.HttpStatusCode == null)
                httpStatusCode = result.ResultType == ResultType.ClientError
                                     ? HttpStatusCode.BadRequest
                                     : HttpStatusCode.InternalServerError;
            else
                httpStatusCode = result.HttpStatusCode.Value;
            return ErrorResult(httpStatusCode, result.ErrorCode, result.ErrorMessage);
        }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="action">The Restful action type (create, show, update, delete, index)</param>
        /// <param name="dto">The dto to return</param>
        protected ActionResult SuccessResult(RestfulAction action, TEntity dto)
        {
            switch(action)
            {
                case RestfulAction.Show:
                    SetResponseStatus(HttpStatusCode.OK);
                    return DynamicResult(dto);
                case RestfulAction.Create:
                    SetResponseStatus(HttpStatusCode.Created);
                    Response.AddHeader("Content-Location", dto.Uri);
                    return null;
                case RestfulAction.Update:
                    SetResponseStatus(HttpStatusCode.Accepted);
                    Response.AddHeader("Content-Location", dto.Uri);
                    return null;
                case RestfulAction.Destroy:
                    SetResponseStatus(HttpStatusCode.OK);
                    return null;
                default:
                    SetResponseStatus(HttpStatusCode.BadRequest, "Unknown action");
                    return null;
            }
        }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="action">The Restful action type (create, show, update, delete, index)</param>
        /// <param name="dtos">The list of dtos to return</param>
        protected ActionResult SuccessResult(RestfulAction action, IEnumerable<TEntity> dtos)
        {
            switch (action)
            {
                case RestfulAction.Index:
                    SetResponseStatus(HttpStatusCode.OK);
                    return DynamicResult(dtos);
                default:
                    SetResponseStatus(HttpStatusCode.BadRequest, "Unknown action");
                    return null;
            }
        }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="action">The Restful action type (create, show, update, delete, index)</param>
        /// <param name="sources">The source objects</param>
        protected ActionResult MapSuccessResult(RestfulAction action, params object[] sources)
        {
            TEntity dto = null;
            if (sources.Any())
                dto = EntityMapper.Map<TEntity>(sources);
            return SuccessResult(action, dto);
        }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="action">The Restful action type (create, show, update, delete, index)</param>
        /// <param name="sources">The source objects</param>
        protected ActionResult MapSuccessResult(RestfulAction action, IEnumerable<object> sources)
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
            ResponseFormatType bodyFormat = new ResponseFormatDecider(Settings).Decide(HttpContext.Request.AcceptTypes, HttpContext.Request.QueryString);
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
                    return View("~/Views/Rest/Html.cshtml",model);
                default:
                    throw new Exception("Unknown response format type");
            }
        }

        protected bool GetParameterAsBool(string paramName)
        {
            return GetParameterAsBool(paramName, false);
        }

        protected bool GetParameterAsBool(string paramName, bool defaultValue)
        {
            string paramValue = Request.Params[paramName];
            bool returnValue = defaultValue;
            bool.TryParse(paramValue, out returnValue);

            return returnValue;
        }

        protected int GetParameterAsInt(string paramName, int defaultValue)
        {
            string paramValue = Request.Params[paramName];
            int returnValue = defaultValue;
            int.TryParse(paramValue, out returnValue);

            return returnValue;
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

            if(!string.IsNullOrEmpty(errorMessage))
                Response.AddHeader("X-GS-ServiceErrorMessage", errorMessage);

            if (errorCode != null)
                Response.AddHeader("X-GS-ServiceErrorCode", errorCode.ToString());
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            SetResponseStatus(HttpStatusCode.InternalServerError, filterContext.Exception.Message);
            filterContext.Result = null;
        }
    }
}
