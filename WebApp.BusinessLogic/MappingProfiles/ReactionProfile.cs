using AutoMapper;
using WebApp.BusinessLogic.DTOs.Reaction;
using WebApp.DataAccess.Entities;

namespace WebApp.BusinessLogic.MappingProfiles;
public class ReactionProfile : Profile
{
    public ReactionProfile()
    {
        _ = this.CreateMap<Reaction, ReactionDto>();
        _ = this.CreateMap<ReactionDto, Reaction>();
    }
}
