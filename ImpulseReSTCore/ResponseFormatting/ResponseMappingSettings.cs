using System.Collections.Generic;

namespace ImpulseReSTCore.ResponseFormatting
{
    public class ResponseMappingSettings
    {
        public ResponseFormatType DefaultResponseFormatType { get; set; }

        public List<ResponseTypeMapping> ResponseTypeMappings { get; set; }

        public static ResponseMappingSettings DefaultSettings
        {
            get
            {
                var settings = new ResponseMappingSettings
                    {
                        DefaultResponseFormatType = ResponseFormatType.Json,
                        ResponseTypeMappings = new List<ResponseTypeMapping>
                            {
                                new ResponseTypeMapping("text/html", ResponseFormatType.Html),
                                new ResponseTypeMapping("application/xhtml+xml", ResponseFormatType.Html),
                                new ResponseTypeMapping("text/plain", ResponseFormatType.Xml),
                                new ResponseTypeMapping("text/xml", ResponseFormatType.Xml),
                                new ResponseTypeMapping("application/xml", ResponseFormatType.Xml),
                                new ResponseTypeMapping("application/json", ResponseFormatType.Json),
                                new ResponseTypeMapping("text/x-json", ResponseFormatType.Json),
                                new ResponseTypeMapping("text/json", ResponseFormatType.Json),
                                new ResponseTypeMapping("application/x-javascript", ResponseFormatType.Jsonp),
                                new ResponseTypeMapping("text/javascript",  ResponseFormatType.Jsonp),
                                new ResponseTypeMapping("text/x-javascript", ResponseFormatType.Jsonp)
                            }
                    };

                return settings;
            }
        }
    }
}
