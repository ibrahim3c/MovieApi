using AutoMapper;
using Movie.DTOs;
using Movie.Models;

namespace Movie.Mapping
{
    public class AuthProfile:Profile
    {
        public AuthProfile()
        {
            CreateMap<UserRegisterationDTO, AppUser>().ForMember(des => des.UserName, src => src.MapFrom(src => src.UserName)).
                ForMember(des => des.Email, src => src.MapFrom(src => src.Email));

        }
    }
}
