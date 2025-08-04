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
            CreateMap<Reports, ReportDTO>();
            CreateMap<ReportDTO, Reports>();
            CreateMap<PostDTO, Posts>();
            CreateMap<Posts, PostDTO>();
            CreateMap<UserDTO, ApplicationUser>();
            CreateMap<ApplicationUser, UserDTO>();
        }
    }
}
