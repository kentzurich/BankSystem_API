using Application.Account.DTO;
using AutoMapper;
using Models;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UserAccount, RegisterAccountDTO>().ReverseMap();
            CreateMap<UserAccount, UserDTO>().ReverseMap();
            CreateMap<UserAccount, AccountDTO>().ReverseMap();
        }
    }
}
