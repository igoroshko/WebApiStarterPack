using AutoMapper;
using System.Linq;
using TestDocker01.Data.Entities;
using TestDocker01.Models;

namespace TestDocker01.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookModel>().ReverseMap();
        }
    }
}
