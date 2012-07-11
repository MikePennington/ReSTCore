using System;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ImpulseReSTCore.DTO
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

                var builder = new UriBuilder(HttpContext.Current.Request.Url);
                builder.Scheme = IsSecureDTO ? "https" : "http";
                builder.Path = System.IO.Path.Combine(Path, HttpUtility.UrlEncode(Id.ToString()));
                if(builder.Port == 80)
                    builder.Port = -1;
                Uri = builder.ToString();
            }
        }

        [XmlAttribute]
        public string Uri { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public abstract string Path { get; }

        [XmlIgnore]
        [JsonIgnore]
        public abstract bool IsSecureDTO { get; }
    }
}
