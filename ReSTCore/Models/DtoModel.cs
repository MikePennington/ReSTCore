using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using ReSTCore.DTO;
using ReSTCore.Util;

namespace ReSTCore.Models
{
    public class DtoModel
    {
        public List<string> Names { get; private set; }
        public string Xsd { get; private set; }

        public DtoModel()
        {
            var dtoTypes = ObjectFinder.FindDtoTypes();
            Xsd = BuildXsd(dtoTypes);

            Names = dtoTypes.Select(x => x.Name).ToList();
        }

        private string BuildXsd(IEnumerable<Type> dtoTypes)
        {
            using (var writer = new StringWriter())
            {
                XmlSchemas xmlSchemas = new XmlSchemas();
                foreach (var type in dtoTypes)
                {
                    XmlReflectionImporter importer = new XmlReflectionImporter();
                    XmlTypeMapping mapping = importer.ImportTypeMapping(type);
                    XmlSchemaExporter xmlSchemaExporter = new XmlSchemaExporter(xmlSchemas);
                    xmlSchemaExporter.ExportTypeMapping(mapping);
                }
                foreach (XmlSchema xmlSchema in xmlSchemas)
                {
                    xmlSchema.Write(writer);                    
                }
                return XElement.Parse(writer.ToString()).ToString();
            }
        }
    }
}
