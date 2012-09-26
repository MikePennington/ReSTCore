using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ReSTCore.ResponseFormatting
{
    public class ResponseFormatDecider
    {
        private readonly ResponseMappingSettings _settings;

        public ResponseFormatDecider(ResponseMappingSettings settings)
        {
            _settings = settings;
        }

        public ResponseFormatType Decide(IEnumerable<string> acceptTypes, NameValueCollection queryString)
        {
            if (queryString != null)
            {
                string callback = queryString.Get("callback");
                if (!string.IsNullOrWhiteSpace(callback))
                    return ResponseFormatType.Jsonp;

                string format = queryString.Get("format");
                if (!string.IsNullOrEmpty(format))
                {
                    format = format.ToLower();
                    switch (format)
                    {
                        case "html":
                            return ResponseFormatType.Html;
                        case "xml":
                            return ResponseFormatType.Xml;
                        case "json":
                            return ResponseFormatType.Json;
                        case "jsonp":
                            return ResponseFormatType.Jsonp;
                    }
                }
            }

            if (acceptTypes != null)
            {
                // Check accept types in the order in which they appear in the list
                foreach (string acceptType in acceptTypes)
                {
                    foreach (var mimeTypeMapping in _settings.ResponseTypeMappings)
                    {
                        if (string.Equals(mimeTypeMapping.MimeType, acceptType, StringComparison.OrdinalIgnoreCase))
                        {
                            return mimeTypeMapping.ResponseFormatType;
                        }
                    }
                }
            }

            return _settings.DefaultResponseFormatType;
        }
    }
}
