using AutoMapper;
using WebApp.BusinessLogic.DTOs.Comment;
using WebApp.DataAccess.Entities;

namespace WebApp.BusinessLogic.MappingProfiles;
public class CommentProfile : Profile
{
    public CommentProfile()
    {
        _ = this.CreateMap<CommentCreateDto, Comment>();
        _ = this.CreateMap<Comment, CommentDto>();
        _ = this.CreateMap<CommentDto, Comment>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PostId, opt => opt.Ignore());
        _ = this.CreateMap<CommentCreateWithoutUserIdDto, CommentCreateDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
        _ = this.CreateMap<CommentUpdateDto, CommentDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}
