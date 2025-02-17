using AutoMapper;
using Motel.DTO;
using Motel.Models;
using System.Net.Sockets;

namespace Motel.Helpers
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ReviewDTO, Reviews>();
            CreateMap<Reviews, ReviewDTO>();
            CreateMap<Reports, ReportsDTO>();
            CreateMap<ReportsDTO, Reports>();
            CreateMap<PostsDTO, Posts>();
            CreateMap<Posts, PostsDTO>();
            CreateMap<UserDTO, ApplicationUser>();
            CreateMap<ApplicationUser, UserDTO>();
        }
    }
}
