using System;

namespace XmlSchemaInferrer
{
    class Program
    {
        static void Main(string[] args)
        {
            new SchemaEngine { FilePath = args[0] }.Run();
        }
    }
}
