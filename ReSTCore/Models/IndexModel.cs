using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReSTCore.Attributes;
using ReSTCore.Controllers;
using ReSTCore.DTO;
using ReSTCore.ResponseFormatting;

namespace ReSTCore.Models
{
    public class IndexModel
    {
        public List<ServiceModel> Services { get; private set; }
        public List<DTO> Dtos { get; private set; }
        public List<ErrorCode> ErrorCodes { get; private set; }
        public List<ResponseFormat> ResponseTypes { get; private set; }
        public string DefaultResponseType { get; private set; }
        public string ServiceName { get; private set; }

        public IndexModel()
        {
            ServiceName = RestCore.Configuration.ServiceName;

            // Load services
            var serviceTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(RestController)));
            Services = new List<ServiceModel>();
            foreach (var type in serviceTypes)
            {
                if (type.Name.StartsWith("TypedRestController"))
                    continue;

                string name = type.Name;
                int index = name.IndexOf("Controller", StringComparison.Ordinal);
                if (index > -1)
                    name = name.Substring(0, index);

                var helpAttr = (HelpAttribute) Attribute.GetCustomAttribute(type, typeof (HelpAttribute));
                var helpText = string.Empty;
                if (helpAttr != null)
                {
                    if(helpAttr.Ignore)
                        continue;
                    helpText = helpAttr.Text;
                }

                Services.Add(new ServiceModel
                                 {
                                     Name = name,
                                     Help = helpText
                                 });
            }

            // Load DTO types
            var dtoTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => IsSubclassOfRawGeneric(typeof (RestDTO<>), type));
            Dtos = new List<DTO>();
            foreach (var type in dtoTypes)
            {
                Dtos.Add(new DTO
                             {
                                 Name = type.Name,
                                 FullName = type.AssemblyQualifiedName
                             });
            }

            // Load response types
            ResponseTypes = new List<ResponseFormat>();
            DefaultResponseType = ResponseMappingSettings.Settings.DefaultResponseFormatType.ToString().ToLower();
            foreach (var responseType in ResponseMappingSettings.Settings.ResponseTypeMappings)
            {
                string formatType = responseType.ResponseFormatType.ToString().ToLower();
                var responseFormat = ResponseTypes.FirstOrDefault(x => x.Type == formatType);
                if (responseFormat != null)
                    responseFormat.AcceptHeaders.Add(responseType.MimeType);
                else
                {
                    ResponseTypes.Add(new ResponseFormat
                                          {
                                              Type = formatType,
                                              AcceptHeaders = new List<string> {responseType.MimeType},
                                              UrlQuery =
                                                  "format=" + formatType
                                          });
                }
            }

            // Load Error Codes
            var errorCodeTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsEnum && type.GetCustomAttributes(typeof(HelpErrorCodesAttribute), true).Length > 0);
            ErrorCodes = new List<ErrorCode>();
            foreach (var errorCodeType in errorCodeTypes)
            {
                FieldInfo[] fields = errorCodeType.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var fieldInfo in fields)
                {
                    ErrorCodes.Add(new ErrorCode{Name = fieldInfo.Name, Code = (int)fieldInfo.GetRawConstantValue()});
                }
            }
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            if (generic == null || toCheck == null)
                return false;
            if (generic == toCheck)
                return false;

            while (toCheck != typeof (object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
                if (toCheck == null)
                    return false;
            }
            return false;
        }
    }

    public class ServiceModel
    {
        public string Name { get; set; }
        public string Help { get; set; }
    }

    public class DTO
    {
        public string Name { get; set; }
        public string FullName { get; set; }
    }

    public class ResponseFormat
    {
        public string Type { get; set; }
        public List<string> AcceptHeaders { get; set; }
        public string UrlQuery { get; set; }
    }

    public class ErrorCode
    {
        public string Name { get; set; }
        public int Code { get; set; }
    }
}
