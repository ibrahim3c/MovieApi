using AutoMapper;
using Movie.DTOs;
using Movie.Models;

namespace Movie.Mapping
{
    public class MovieProfile:Profile
    {
        public MovieProfile()
        {
            CreateMap<MOvie, MovieDetails>().ForMember(des=>des.genre, src=>src.MapFrom(src=>src.Genre.Name));
        }
    }
}
