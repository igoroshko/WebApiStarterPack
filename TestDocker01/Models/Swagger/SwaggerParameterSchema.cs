using System;

namespace TestDocker01.Models.Swagger
{
    public class SwaggerParameterSchema
    {
        public string Type { get; set; }

        public string Format { get; set; }

        public string Ref { get; set; }

        internal string LocalType()
        {
            if (Type == "integer")
            {
                return "int";
            }

            if (Type == "boolean")
            {
                return "bool";
            }

            if (Type == "string")
            {
                return "string";
            }

            return Type + "." + Format;
        }
    }
}
