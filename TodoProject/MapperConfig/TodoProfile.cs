using AutoMapper;
using TodoProject.Models;
using TodoProject.ModelsDTO;

namespace TodoProject.MapperConfig
{
    public class TodoProfile : Profile
    {
        public TodoProfile() 
        {
            CreateMap<TodoDTO, Todo>()
                .ForMember(dst => dst.DateTask,
                opt => opt.MapFrom(src => $"{DateTime.Now:dd-MM-yyyy}"));
        }
    }
}
