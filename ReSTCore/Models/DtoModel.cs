using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using ReSTCore.DTO;

namespace ReSTCore.Models
{
    public class DtoModel
    {
        public bool IsValid { get; private set; }
        public string Name { get; private set; }
        public string Xsd { get; private set; }

        public DtoModel(string name)
        {
            Type dtoType = Type.GetType(name);
            if (dtoType == null)
            {
                Name = name;
                return;
            }

            IsValid = true;
            Name = dtoType.Name;
            
            Xsd = ExtractXsdFromType(dtoType);
        }

        private string ExtractXsdFromType(Type type)
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
    }
}
