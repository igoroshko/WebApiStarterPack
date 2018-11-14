using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestDocker01.Common;
using TestDocker01.Models;
using TestDocker01.Models.Swagger;

namespace TestDocker01.Controllers
{
    /// <summary>
    /// The base controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private IList<string> _validationErrors = new List<string> { };

        private bool _isNotFound = false;

        protected readonly IMapper _mapper;

        protected readonly IConfiguration _config;

        public BaseController()
        {
        }

        public BaseController(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected TResult Map<TResult>(object source)
        {
            return _mapper.Map<TResult>(source);
        }

        protected IList<TModel> MapList<TModel>(object source)
        {
            return _mapper.Map<IList<TModel>>(source);
        }

        protected SearchResult<TModel> MapSearchResult<TEntity, TModel>(SearchResult<TEntity> result)
        {
            return new SearchResult<TModel>
            {
                Metadata = result.Metadata,
                Items = MapList<TModel>(result.Items)
            };
        }

        protected void AddValidationError(string error = null)
        {
            _validationErrors.Add(error);
        }

        protected void AddIdNotPositiveError(string error = null)
        {
            _validationErrors.Add(error);
        }

        protected void AddNotFoundError(string error = null)
        {
            _isNotFound = true;
        }

        protected bool HasErrors()
        {
            return _isNotFound || _validationErrors.Count > 0;
        }

        protected IActionResult GetResult()
        {
            if (_isNotFound)
            {
                return NoContent();
            }

            if (_validationErrors.Count > 0)
            {
                return BadRequest();
            }

            return Ok();
        }

        protected IActionResult GetResult<TResult>(TResult result)
        {
            if (_isNotFound)
            {
                return NoContent();
            }

            if (_validationErrors.Count > 0)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        #region postman test

        [HttpGet("postman")]
        public dynamic GetPostman(string filePath = null)
        {
            string baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/";
            dynamic result = new
            {
                variables = new string[0],
                info = new
                {
                    name = "api_tests_" + DateTime.Now.ToString("hh-mm-ss"),
                    _postman_id = Guid.NewGuid().ToString(),
                    schema = "https://schema.getpostman.com/json/collection/v2.0.0/collection.json"
                },
                item = new List<dynamic> { }
            };

            // get all controllers derived from BaseController
            var thisType = typeof(BaseController);
            var assemblyTypes = thisType.Assembly.GetTypes();
            var controllerTypes = assemblyTypes.Where(x => !x.IsAbstract && thisType.IsAssignableFrom(x)).ToList();

            //     foreach controller get all public methods with HttpXxx attributes
            foreach (var type in controllerTypes)
            {
                // get name
                string controllerName = type.Name.EndsWith("Controller") ? type.Name.Substring(0, type.Name.Length - "Controller".Length) : type.Name;
                dynamic controllerItem = new
                {
                    name = controllerName,
                    item = new List<dynamic> { }
                };
                result.item.Add(controllerItem);

                var methods = type.GetMethods()/*.Where(x => x.DeclaringType == type)*/.ToList();
                foreach (var method in methods)
                {
                    var httpAttr = method.GetCustomAttributes(typeof(HttpMethodAttribute), true).Cast<HttpMethodAttribute>().FirstOrDefault();
                    if (httpAttr != null)
                    {
                        string httpMethod = httpAttr.HttpMethods.FirstOrDefault();
                        if (httpMethod != null)
                        {
                            string body = null;
                            string uri = baseUri + controllerName;
                            if (!string.IsNullOrEmpty(httpAttr.Template))
                            {
                                uri += "/" + httpAttr.Template;
                            }

                            foreach (var param in method.GetParameters())
                            {
                                var pType = param.ParameterType;
                                if (pType == typeof(string) || pType.IsValueType || IsNullableType(pType))
                                {
                                    var sampleValue = GetSampleValue(pType);
                                    string namePlaceholder = "{" + param.Name + "}";
                                    if (uri.Contains(namePlaceholder))
                                    {
                                        uri = uri.Replace(namePlaceholder, sampleValue.ToString());
                                    }
                                    else
                                    {
                                        string del = uri.Contains("?") ? "&" : "?";
                                        uri += $"{del} {param.Name}={sampleValue}";
                                    }
                                }
                                else
                                {
                                    if (httpMethod == "POST" || httpMethod == "PUT")
                                    {
                                        var obj = GetSampleObject(pType, 3);
                                        body = JsonConvert.SerializeObject(obj, Formatting.Indented, Util.SerializerWithEnumConverter);
                                    }
                                    else
                                    {
                                        throw new NotSupportedException("complex object for non-POST/PUT method.");
                                    }
                                }
                            }

                            dynamic methodItem = new
                            {
                                name = $"{httpMethod} /{httpAttr.Template}",
                                request = new
                                {
                                    url = uri,
                                    method = httpMethod,
                                    header = new[]
                                    {
                                        new
                                        {
                                            key = "Content-Type",
                                            value = "application/json",
                                            description = ""
                                        }
                                    },
                                    body = new
                                    {
                                        mode = body != null ? "raw" : null,
                                        raw = body
                                    }
                                }
                            };

                            controllerItem.item.Add(methodItem);
                        }
                    }
                }
            }

            if (filePath != null)
            {
                string serialized = JsonConvert.SerializeObject(result, Formatting.Indented, Util.SerializerWithEnumConverter);
                System.IO.File.WriteAllText(filePath, serialized);
            }

            return result;
        }

        private object GetSampleObject(Type type, int level)
        {
            if (level == 0)
            {
                return null;
            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                Type typeArgument = type.GenericTypeArguments[0];
                var listType = typeof(List<>).MakeGenericType(typeArgument);
                var list = Activator.CreateInstance(listType);
                MethodInfo addMethod = listType.GetMethod("Add");
                for (int i = 0; i < 2; i++)
                {
                    object item = typeArgument == typeof(string) || typeArgument.IsValueType || IsNullableType(typeArgument)
                        ? GetSampleValue(typeArgument)
                        : GetSampleObject(typeArgument, level);
                    addMethod.Invoke(list, new[] { item });
                }
                return list;
            }

            object result = Activator.CreateInstance(type);
            IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite);

            foreach (PropertyInfo property in properties)
            {
                object value = property.PropertyType == typeof(string) || property.PropertyType.IsValueType || IsNullableType(property.PropertyType)
                    ? GetSampleValue(property.PropertyType)
                    : GetSampleObject(property.PropertyType, level - 1);

                property.SetValue(result, value);
            }

            return result;
        }

        private object GetSampleValue(Type type)
        {
            if (IsNullableType(type))
            {
                type = type.GenericTypeArguments[0];
            }

            if (type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(byte))
            {
                return 1;
            }

            if (type == typeof(double) ||
                type == typeof(decimal))
            {
                return 1.5;
            }

            if (type == typeof(string))
            {
                return "testString";
            }

            if (type == typeof(bool))
            {
                return true;
            }

            if (type == typeof(DateTime))
            {
                return DateTime.Now.ToLongTimeString();
            }

            if (type.IsEnum)
            {
                var values = Enum.GetValues(type);
                return values.GetValue(0);
            }

            throw new NotSupportedException($"{type.Name} not supported");
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        #endregion

        #region swagger

        [HttpGet("swagger")]
        public void FromSwagger(string filePath, string outputPath)
        {
            string contents = System.IO.File.ReadAllText(filePath);
            var root = JObject.Parse(contents);
            var paths = root["paths"];
            var dict = new Dictionary<string, IList<SwaggerPath>>();
            foreach (var property in (paths as JObject).Properties())
            {
                string controllerName = property.Name.Trim('/');
                string path = controllerName;
                int index = controllerName.IndexOf('/');
                if (index > 0)
                {
                    controllerName = controllerName.Substring(0, index);
                }

                path = path.Substring(controllerName.Length);

                IList<SwaggerPath> coll;
                if (!dict.TryGetValue(controllerName, out coll))
                {
                    coll = new List<SwaggerPath>();
                    dict[controllerName] = coll;
                }

                var swaggerPath = property.Value.ToObject<SwaggerPath>();
                swaggerPath.Path = path;
                coll.Add(swaggerPath);
            }



            var components = new List<SwaggerComponent>();
            foreach (var property in (root["components"]["schemas"] as JObject).Properties())
            {
                var component = property.Value.ToObject<SwaggerComponent>();
                component.Name = property.Name;
                components.Add(component);
            }

            foreach (var item in components)
            {
                item.CreateEntity(Path.Combine(outputPath, "models"), "SwaggerModels");
            }
        }

        #endregion
    }
}
