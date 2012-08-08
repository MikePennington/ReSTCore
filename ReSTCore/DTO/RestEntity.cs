using System;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;
using ReSTCore.Util;

namespace ReSTCore.DTO
{
    public abstract class RestEntity<T>
    {
        private T _id;

        [XmlAttribute]
        public T Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                if (Id == null)
                    return;

                if (RestCore.ServiceBaseUri != null)
                {
                    Uri = RestCore.ServiceBaseUri.Combine(Path).CombineUri(HttpUtility.UrlEncode(Id.ToString()));
                }
                else
                {
                    var builder = HttpContext.Current == null ? new UriBuilder() : new UriBuilder(HttpContext.Current.Request.Url);
                    builder.Scheme = "http";
                    builder.Path = System.IO.Path.Combine(Path, HttpUtility.UrlEncode(Id.ToString()));
                    if (builder.Port == 80)
                        builder.Port = -1;
                    Uri = builder.ToString();
                }
            }
        }

        [XmlAttribute]
        public string Uri { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public abstract string Path { get; }
    }
}
