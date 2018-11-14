namespace TestDocker01.Models.Swagger
{
    public class SwaggerMethodParameter
    {
        public string Name { get; set; }

        public SwaggerParameterType In { get; set; }

        public bool Required { get; set; }

        public SwaggerParameterSchema Schema { get; set; }
    }
}
