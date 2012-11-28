﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
﻿using ReSTCore.Attributes;
﻿using ReSTCore.Util;

namespace ReSTCore.Models
{
    public class DtoModel
    {
        public bool IsValid { get; private set; }
        public string Name { get; private set; }
        public string Xsd { get; private set; }
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
    }
}
