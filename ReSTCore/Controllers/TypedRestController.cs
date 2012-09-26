using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ReSTCore.Attributes;
using ReSTCore.DTO;
using ReSTCore.Mapping;
using ReSTCore.Models;
using ReSTCore.Routing;
using ReSTCore.Util;

namespace ReSTCore.Controllers
{
    /// <typeparam name="TId">The type of the ID of this object controlled by the controller (e.g. 0 int or string)</typeparam>
    /// <typeparam name="TEntity">The type of the object controlled by this controller</typeparam>
    public class TypedRestController<TId, TEntity> : RestController
        where TEntity : RestDTO<TId>
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
        [Help(Ignore = true)]
        public ActionResult Help()
        {
            var model = new HelpModel(this);
            return View("~/Views/RestCore/Help.cshtml", model);
        }

        protected bool ValidateCreate(RestDTO<TId> entity)
        {
            if (entity == null)
            {
                SetResponseStatus(HttpStatusCode.BadRequest, "Object to create must be provided");
                return false;
            }

            return ValidateEntity(entity);
        }

        protected bool ValidateUpdate(TId id, RestDTO<TId> entity)
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

        protected ActionResult HandleGetResult<T>(T entity) where T : class
        {
            return HandleGetResult<T>(entity, new HandleResultProperties());
        }

        protected ActionResult HandleGetResult<T>(T entity, HandleResultProperties properties) where T : class
        {
            Result<T> result;
            if (entity == null)
                result = new Result<T> {HttpStatusCode = HttpStatusCode.NotFound, ResultType = ResultType.ClientError};
            else
                result = new Result<T> {ResultType = ResultType.Success, Entity = entity};
            return HandleResult<T>(RestfulAction.Show, result, properties);
        }

        protected ActionResult HandleResult<T>(RestfulAction action, Result<T> result) where T : class
        {
            return HandleResult<T>(action, result, new HandleResultProperties());
        }

        protected ActionResult HandleResult<T>(RestfulAction action, Result<T> result, HandleResultProperties properties) where T : class
        {
            if (result.ResultType == ResultType.Success)
                if (properties.NeedsMapping)
                    return MapSuccessResult(action, properties, result.Entity);
                else
                    return SuccessResult(action, properties, result.Entity);

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
        /// <param name="properties">Properties defining how the result is formatted</param>
        /// <param name="dto">The dto to return</param>
        protected ActionResult SuccessResult<T>(RestfulAction action, HandleResultProperties properties, T dto) where T : class
        {
            string uri = string.Empty;
            if (dto != null && dto.GetType().IsSubclassOf(typeof(RestDTO<TId>)))
                uri = (dto as RestDTO<TId>).Uri;
            
            switch(action)
            {
                case RestfulAction.Index:
                case RestfulAction.Show:
                    SetResponseStatus(HttpStatusCode.OK);
                    return DynamicResult(dto);
                case RestfulAction.Create:
                    SetResponseStatus(HttpStatusCode.Created);
                    Response.AddHeader("Content-Location", uri);
                    return properties.IncludeBodyInNonGetRequest ? DynamicResult(dto) : null;
                case RestfulAction.Update:
                    SetResponseStatus(HttpStatusCode.Accepted);
                    Response.AddHeader("Content-Location", uri);
                    return properties.IncludeBodyInNonGetRequest ? DynamicResult(dto) : null;
                case RestfulAction.Delete:
                    SetResponseStatus(HttpStatusCode.OK);
                    return properties.IncludeBodyInNonGetRequest ? DynamicResult(dto) : null;
                default:
                    SetResponseStatus(HttpStatusCode.BadRequest, "Unknown action");
                    return properties.IncludeBodyInNonGetRequest ? DynamicResult(dto) : null;
            }
        }

        /// <summary> Returns the current ActionResult for the given source objects</summary>
        /// <param name="action">The Restful action type (create, show, update, delete, index)</param>
        /// <param name="properties"></param>
        /// <param name="sources">The source objects</param>
        private ActionResult MapSuccessResult(RestfulAction action, HandleResultProperties properties, params object[] sources)
        {
            TEntity dto = null;
            if (sources.Any())
                dto = EntityMapper.Map<TEntity>(sources);
            return SuccessResult(action, properties, dto);
        }
    }
}
