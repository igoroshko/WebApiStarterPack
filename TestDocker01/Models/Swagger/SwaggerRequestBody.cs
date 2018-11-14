using Newtonsoft.Json.Linq;

namespace TestDocker01.Models.Swagger
{
    public class SwaggerRequestBody
    {
        public JObject Content { get; set; }

        public string TypeRef
        {
            get
            {
                if (Content == null)
                {
                    return null;
                }

                var contentType = Content["application/json"];
                if (contentType == null)
                {
                    return null;
                }

                var schema = contentType["schema"];
                if (schema == null)
                {
                    return null;
                }

                return schema["$ref"].ToString();
            }
        }
    }
}
