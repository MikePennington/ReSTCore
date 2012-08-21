using System.Collections.Generic;

namespace ReSTCore.ResponseFormatting
{
    public class ResponseMappingSettings
    {
        public ResponseFormatType DefaultResponseFormatType { get; set; }

        public List<ResponseTypeMapping> ResponseTypeMappings { get; set; }

        private static ResponseMappingSettings _settings;

        public static ResponseMappingSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = DefaultSettings;
                return _settings;
            }
            set { _settings = value; }
        }

        public static void ResetToDefaultSettings()
        {
            Settings = DefaultSettings;
        }

        private static ResponseMappingSettings DefaultSettings
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
                                new ResponseTypeMapping("application/javascript", ResponseFormatType.Jsonp),
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
