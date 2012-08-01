using System.Web.Mvc;
using System.Xml.Serialization;

namespace ReSTCore.ActionResults
{
    public class XmlResult : ActionResult
    {
        private readonly object _objectToSerialize;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResult"/> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to XML.</param>
        public XmlResult(object objectToSerialize)
        {
            _objectToSerialize = objectToSerialize;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object ObjectToSerialize
        {
            get { return _objectToSerialize; }
        }

        /// <summary>
        /// Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (_objectToSerialize == null)
                return;

            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "text/xml";

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var xs = new XmlSerializer(_objectToSerialize.GetType());
            xs.Serialize(context.HttpContext.Response.Output, _objectToSerialize, ns);
        }
    }
}
