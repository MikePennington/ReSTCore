﻿using System;
﻿using System.Collections;
﻿using System.Collections.Generic;
﻿using System.IO;
using System.Linq;
﻿using System.Reflection;
﻿using System.Xml.Linq;
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

            // Properties with public setters
            IEnumerable<PropertyInfo> properties = t.GetProperties(
                BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetSetMethod() != null).Where(p => p.GetSetMethod().GetParameters().Count() == 1);
            foreach (var propertyInfo in properties)
            {
                propertyInfo.SetValue(obj, GetDefaultForType(propertyInfo.PropertyType), null);
            }

            return obj;
        }

        private object GetDefaultForType(Type t)
        {
            try
            {
                if (t == typeof(char) || t == typeof(char?))
                {
                    return '?';
                }
                else if (t == typeof(string))
                {
                    return "?";
                }
                else if (t == typeof(Guid) || t == typeof(Guid?))
                {
                    return Guid.NewGuid();
                }
                else if (t == typeof(DateTime) || t == typeof(DateTime?))
                {
                    return DateTime.Now;
                }
                else if (t.IsValueType)
                {
                    return Activator.CreateInstance(t);
                }
                else if (t.IsArray)
                {
                    Array array = Array.CreateInstance(t.GetElementType(), 2);
                    var defaultForType = GetDefaultForType(t.GetElementType());
                    array.SetValue(defaultForType, 0);
                    array.SetValue(defaultForType, 1);
                    return array;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(t))
                {
                    IList list = (IList)Activator.CreateInstance(t);
                    Type[] generics = t.GetGenericArguments();
                    var defaultForType = generics.Any() ? GetDefaultForType(generics[0]) : new object();
                    list.Add(defaultForType);
                    list.Add(defaultForType);
                    return list;
                }
                else
                {
                    object innerObj = CreateEmptyObject(t);
                    return innerObj;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
