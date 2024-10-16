using WebApp.BusinessLogic.DTOs.Post;

namespace WebApp.WebApi.Interfaces;

public interface IPostGenerator
{
    Task<CreatePostDto> GeneratePost(CreatePostOnlyTitleRequestDto createPostOnlyTitleRequestDto);
}
