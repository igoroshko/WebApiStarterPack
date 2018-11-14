using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TestDocker01.Models.Swagger
{
    public class SwaggerComponent
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public JObject Properties { get; set; }

        public void CreateEntity(string basePath, string nameSpace)
        {
            string filePath = Path.Combine(basePath, Name + ".cs");
            using (var sw = new StreamWriter(filePath))
            {
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine();
                sw.WriteLine("namespace " + nameSpace);
                sw.WriteLine("{");
                sw.WriteLine("    public class " + Name);
                sw.WriteLine("    {");

                foreach (var item in Properties)
                {
                    var name = item.Key;
                    name = char.ToUpper(name[0]) + name.Substring(1);
                    // if ((item.Value as JToken).Contains()
                    var par = item.Value.ToObject<SwaggerParameterSchema>();
                    sw.WriteLine($"        public {par.LocalType()} {name} {{ get; set; }}");
                }

                sw.WriteLine("    }");
                sw.WriteLine("}");

            }
        }
    }
}
