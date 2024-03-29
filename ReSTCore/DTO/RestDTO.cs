﻿using System;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;
using ReSTCore.Util;

namespace ReSTCore.DTO
{
    public abstract class RestDTO<T>
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

                Uri = BuildUri();
            }
        }

        [XmlAttribute]
        public string Uri { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        protected string BaseUri
        {
            get
            {
                if (RestCore.Configuration != null && RestCore.Configuration.ServiceBaseUri != null)
                {
                    return RestCore.Configuration.ServiceBaseUri.Combine(Path);
                }
                else
                {
                    Uri uri = HttpContext.Current == null ? new Uri("http://localhost") : HttpContext.Current.Request.Url;
                    var builder = new UriBuilder
                                      {
                                          Scheme = uri.Scheme,
                                          Host = uri.Host,
                                          Path = Path,
                                          Port = uri.Port
                                      };
                    if (builder.Port == 80)
                        builder.Port = -1;
                    return builder.ToString();
                }
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public abstract string Path { get; }

        protected virtual string BuildUri()
        {
            return string.Format("{0}/{1}", BaseUri, Id);
        }
    }
}
