using AutoMapper;
using WebApp.BusinessLogic.DTOs.Tag;
using WebApp.DataAccess.Entities;

namespace WebApp.BusinessLogic.MappingProfiles;
public class TagProfile : Profile
{
    public TagProfile()
    {
        _ = this.CreateMap<TagDto, Tag>();
        _ = this.CreateMap<Tag, TagDto>();
    }
}
