using AutoMapper;
using WebApp.BusinessLogic.DTOs.Post;
using WebApp.BusinessLogic.DTOs.Tag;
using WebApp.DataAccess.Entities;

namespace WebApp.BusinessLogic.MappingProfiles;
public class PostProfile : Profile
{
    public PostProfile()
    {
        _ = this.CreateMap<CreatePostDto, Post>();
        _ = this.CreateMap<Post, PostDto>()
             .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
        _ = this.CreateMap<PostDto, Post>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore());

        _ = this.CreateMap<CreatePostWithoutUserIdDto, CreatePostDto>()
               .ForMember(dest => dest.UserId, opt => opt.Ignore());

        _ = this.CreateMap<PostUpdateDto, PostDto>();
        _ = this.CreateMap<PostUpdateDto, Post>();
        _ = this.CreateMap<PostDto, Post>();

        _ = this.CreateMap<string, TagDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src));

        _ = this.CreateMap<OpenAiGenerateResponseDto, CreatePostDto>()
                .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.TopicId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        _ = this.CreateMap<CreatePostOnlyTitleRequestDto, CreatePostDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
    }
}
