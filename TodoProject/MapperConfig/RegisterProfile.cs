using AutoMapper;
using TodoProject.Models;
using TodoProject.ModelsDTO;

namespace TodoProject.MapperConfig
{
    public class RegisterProfile : Profile
    {
        public RegisterProfile() 
        {
            CreateMap<RegisterDTO, User>()
                .ForMember(dst => dst.PasswordHash,
                opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));
        }
    }
}
