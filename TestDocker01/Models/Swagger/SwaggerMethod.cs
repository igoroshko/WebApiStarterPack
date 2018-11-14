using System;
using System.Collections.Generic;
using System.IO;

namespace TestDocker01.Models.Swagger
{
    public class SwaggerMethod
    {
        public string Template { get; set; }

        public IList<SwaggerMethodParameter> Parameters { get; set; }

        public SwaggerRequestBody RequestBody { get; set; }

        internal void WriteMethods(StreamWriter sw, string path, string method)
        {
            sw.WriteLine($"        [Http{method}(\"{path}\")]");
            sw.WriteLine($"        public void ");
        }
    }
}
