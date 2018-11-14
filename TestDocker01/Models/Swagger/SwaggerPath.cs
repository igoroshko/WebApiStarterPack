namespace TestDocker01.Models.Swagger
{
    public class SwaggerPath
    {
        public string Path { get; set; }

        public SwaggerMethod Get { get; set; }

        public SwaggerMethod Post { get; set; }

        public SwaggerMethod Put { get; set; }

        public SwaggerMethod Delete { get; set; }

        public SwaggerMethod Patch { get; set; }

        public void WriteMethods(StreamWriter sw)
        {

        }
    }
}
