using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;
using ReSTCore.DTO;

namespace ReSTCore.ActionResults
{
    public class XmlResult : ActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResult"/> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to XML.</param>
        public XmlResult(object objectToSerialize)
        {
            ObjectToSerialize = objectToSerialize;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object ObjectToSerialize { get; private set; }

        /// <summary>
        /// Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (ObjectToSerialize == null)
                return;

            context.HttpContext.Response.ContentType = "text/xml";

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var xs = new XmlSerializer(ObjectToSerialize.GetType());
            xs.Serialize(context.HttpContext.Response.Output, ObjectToSerialize, ns);
        }

        [XmlRoot("Response")]
        public class StringDTOSerializer
        {
            public string Value { get; set; }
        }
    }
}
