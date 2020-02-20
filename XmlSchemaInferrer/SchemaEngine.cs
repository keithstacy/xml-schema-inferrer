using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace XmlSchemaInferrer
{
    public class SchemaEngine
    {
        public string FilePath { get; set; }
        public void Run()
        {
            MemoryStream stream = new MemoryStream();
            XmlReader reader = XmlReader.Create(FilePath);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            XmlSchemaInference schema = new XmlSchemaInference();
            schemaSet = schema.InferSchema(reader);
            foreach (XmlSchema item in schemaSet.Schemas())
            {
                item.Write(stream);
            }
            File.WriteAllBytes(FilePath + ".xsd", stream.ToArray());
        }
    }
}
