﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
﻿using Newtonsoft.Json;
﻿using Newtonsoft.Json.Converters;
﻿using ReSTCore.Attributes;
﻿using ReSTCore.Util;

namespace ReSTCore.Models
{
    public class DtoModel
    {
        public bool IsValid { get; private set; }
        public string Name { get; private set; }
        public string Xsd { get; private set; }
        public string Xml { get; private set; }
        public string Json { get; private set; }
        public string Description { get; private set; }

        public DtoModel(string fullName)
        {
            fullName = UriHelper.FromBase64UrlString(fullName);
            Type dtoType = Type.GetType(fullName);
            if (dtoType == null)
            {
                Name = fullName;
                return;
            }

            IsValid = true;
            Name = dtoType.Name;

            Xsd = ConstructXsdFromType(dtoType);
            Xml = ConstructExampleXmlFromType(dtoType);
            Json = ConstructExampleJsonFromType(dtoType);

            var helpAttr = (HelpAttribute)Attribute.GetCustomAttribute(dtoType, typeof(HelpAttribute), false);
            if (helpAttr != null)
                Description = helpAttr.Text;
        }

        private string ConstructXsdFromType(Type type)
        {
            try
            {
                XmlReflectionImporter importer = new XmlReflectionImporter();
                XmlTypeMapping mapping = importer.ImportTypeMapping(type);
                XmlSchemas xmlSchemas = new XmlSchemas();
                XmlSchemaExporter xmlSchemaExporter = new XmlSchemaExporter(xmlSchemas);

                using (var writer = new StringWriter())
                {
                    xmlSchemaExporter.ExportTypeMapping(mapping);
                    xmlSchemas[0].Write(writer);
                    return XElement.Parse(writer.ToString()).ToString();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private string ConstructExampleXmlFromType(Type t)
        {
            try
            {
                var dto = CreateEmptyObject(t);
                
                var ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                var xs = new XmlSerializer(t);
                using (var writer = new StringWriter())
                {
                    xs.Serialize(writer, dto, ns);
                    return writer.ToString();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private string ConstructExampleJsonFromType(Type t)
        {
            try
            {
                var dto = CreateEmptyObject(t);

                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.Converters.Add(new IsoDateTimeConverter());
                return JsonConvert.SerializeObject(dto, Formatting.None, serializerSettings);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private object CreateEmptyObject(Type t)
        {
            var obj = Activator.CreateInstance(t);
            foreach (var propertyInfo in t.GetProperties())
            {
                if(!propertyInfo.CanWrite)
                    continue;
                if (propertyInfo.PropertyType == typeof(short) || propertyInfo.PropertyType == typeof(short?)
                    || propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?)
                    || propertyInfo.PropertyType == typeof(long) || propertyInfo.PropertyType == typeof(long?)
                    || propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(double?)
                    || propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?))
                {
                    continue;
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(obj, string.Empty, null);
                }
                else if (propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
                {
                    propertyInfo.SetValue(obj, Guid.NewGuid(), null);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                {
                    propertyInfo.SetValue(obj, DateTime.Now, null);
                }
                else if (!propertyInfo.GetType().IsPrimitive)
                {
                    object innerObj = CreateEmptyObject(propertyInfo.GetType());
                    propertyInfo.SetValue(obj, innerObj, null);
                }
            }
            return obj;
        }
    }
}
