using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using ImpulseReSTCore.ActionResults;
using ImpulseReSTCore.Entity;
using ImpulseReSTCore.Mapping;
using ImpulseReSTCore.ResponseFormatting;

namespace ImpulseReSTCore
{
    /// <typeparam name="TId">The type of the ID of this object controlled by the controller (e.g. 0 int or string)</typeparam>
    /// <typeparam name="TEntity">The type of the object controlled by this controller</typeparam>
    public class BaseController<TId, TEntity> : Controller
        where TEntity : class, IRestEntity<TId>
    {
        public BaseController()
        {
            Settings = ResponseMappingSettings.DefaultSettings;
        }

        /// <summary>
        /// Lists objects
        /// Method: GET
        /// Uri:    /[controller]
        /// </summary>
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
        public virtual ActionResult Help()
        {
            return View("");
        }

        protected bool ValidateCreate(IRestEntity<TId> entity)
        {
            if (entity == null)
            {
                SetBadRequest("Object to create must be provided");
                return false;
            }

            return ValidateEntity(entity);
        }

        protected bool ValidateUpdate(TId id, IRestEntity<TId> entity)
        {
            if (entity == null)
            {
                SetBadRequest("Object to update must be provided");
                return false;
            }

            if (id == null)
            {
                SetBadRequest("ID must be povided on update");
                return false;
            }

            if (id is string && string.IsNullOrWhiteSpace(id.ToString()))
            {
                SetBadRequest("ID must not be empty");
                return false;
            }

            return ValidateEntity(entity);
        }

        private bool ValidateEntity(IRestEntity<TId> entity)
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
                SetBadRequest(message.ToString());
                return false;
            }
            return true;
        }

        protected ResponseMappingSettings Settings { get; set; }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="sources">The source objects</param>
        protected ActionResult MapResult(params object[] sources)
        {
            if (!sources.Any() || sources.Count() == 1 && sources[0] == null)
            {
                SetNotFound();
                return null;
            }

            var obj = EntityMapper.Map<TEntity>(sources);
            return GetResult(obj);
        }

        protected ActionResult GetResult(TEntity entity)
        {
            SetResponseStatus(HttpStatusCode.Created, HttpStatusCode.Created.ToString());
            Response.AddHeader("Content-Location", entity.Uri);

            return DynamicResult(entity);
        }

        protected  ActionResult NotFoundResult()
        {
            return new HttpNotFoundResult(string.Format("{0} was not found", typeof(TEntity).Name));
        }

        protected ActionResult NotFoundResult(TId id)
        {
            return new HttpNotFoundResult(string.Format("{0} with id {1} was not found", typeof(TEntity).Name, id));
        }

        protected ActionResult CreatedResult(TEntity entity)
        {
            SetResponseStatus(HttpStatusCode.Created, HttpStatusCode.Created.ToString());
            Response.AddHeader("Content-Location", entity.Uri);
            return null;
        }

        protected ActionResult UpdatedResult(TEntity entity)
        {
            SetResponseStatus(HttpStatusCode.Accepted, HttpStatusCode.Accepted.ToString());
            Response.AddHeader("Content-Location", entity.Uri);
            return null;
        }

        protected ActionResult DeletedResult()
        {
            SetResponseStatus(HttpStatusCode.OK, HttpStatusCode.OK.ToString());
            return null;
        }

        protected ActionResult DynamicResult(TEntity entity)
        {
            ResponseFormatType bodyFormat = new ResponseFormatDecider(Settings).Decide(HttpContext.Request.AcceptTypes, HttpContext.Request.QueryString);
            switch (bodyFormat)
            {
                case ResponseFormatType.Xml:
                case ResponseFormatType.Html:
                    return new XmlResult(entity);
                case ResponseFormatType.Json:
                    return Json(entity, JsonRequestBehavior.AllowGet);
                case ResponseFormatType.Jsonp:
                    return new JsonpResult(entity);
                /*case ResponseFormatType.Html:
                    if (!string.IsNullOrWhiteSpace(viewName))
                        return View(viewName, data);
                    else
                        return DynamicResult(_settings.DefaultResponseFormatType, data, viewName);*/
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
        /// Not found
        /// 10.4.5 404 Not Found
        /// </summary>
        protected void SetNotFound()
        {
            SetResponseStatus(HttpStatusCode.NotFound, "Resource not found");
        }

        /// <summary>
        /// The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications. 
        /// 10.4.1 400 Bad Request
        /// </summary>
        protected void SetBadRequest(string description)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            Response.StatusDescription = description;
            Response.Write(Response.StatusCode + ": " + Response.StatusDescription);
        }

        /// <summary>
        /// This method will set the Method Not Allowed status code - Because an action is not available in the given context
        /// 10.4.6 405 Method Not Allowed
        /// </summary>
        protected void SetMethodNotAllowed()
        {
            Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            Response.StatusDescription = string.Format("Action {0} is not an acceptable action for this resource.", ControllerContext.RouteData.GetRequiredString("action"));
            Response.TrySkipIisCustomErrors = true;
            Response.SuppressContent = true;
        }

        /// <summary>
        /// This method will accept a status code and message and modify the response according to the RFC2616 specification
        /// </summary>
        private void SetResponseStatus(HttpStatusCode httpStatusCode, string message)
        {
            Response.StatusCode = (int)httpStatusCode;
            Response.StatusDescription = message;
            Response.TrySkipIisCustomErrors = true;

            if (httpStatusCode == HttpStatusCode.NotModified || httpStatusCode == HttpStatusCode.NotFound
                || httpStatusCode == HttpStatusCode.MethodNotAllowed
                || httpStatusCode == HttpStatusCode.UnsupportedMediaType
                || httpStatusCode == HttpStatusCode.MovedPermanently)
            {
                Response.SuppressContent = true;
            }
        }
    }
}
