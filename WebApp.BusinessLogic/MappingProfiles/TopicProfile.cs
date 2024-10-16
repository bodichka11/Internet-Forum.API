using AutoMapper;
using WebApp.BusinessLogic.DTOs.Topic;
using WebApp.DataAccess.Entities;

namespace WebApp.BusinessLogic.MappingProfiles;
public class TopicProfile : Profile
{
    public TopicProfile()
    {
        _ = this.CreateMap<Topic, TopicDto>();
        _ = this.CreateMap<TopicDto, Topic>()
            .ForMember(dest => dest.Posts, opt => opt.Ignore());
        _ = this.CreateMap<TopicCreateDto, Topic>();

        _ = this.CreateMap<TopicUpdateDto, TopicDto>();
        _ = this.CreateMap<TopicUpdateDto, Topic>();
    }
}
