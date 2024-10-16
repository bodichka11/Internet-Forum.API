using AutoMapper;
using Newtonsoft.Json;
using WebApp.BusinessLogic.DTOs.Post;
using WebApp.WebApi.Interfaces;

namespace WebApp.WebApi.Services;

public class PostGenerator : IPostGenerator
{
    private readonly IOpenAiSettings openAiSettings;
    private readonly ICompletionService completionService;
    private readonly IMapper mapper;

    public PostGenerator(IOpenAiSettings openAiSettings, ICompletionService completionService, IMapper mapper)
    {
        this.openAiSettings = openAiSettings;
        this.completionService = completionService;
        this.mapper = mapper;
    }

    public async Task<CreatePostDto> GeneratePost(CreatePostOnlyTitleRequestDto createPostOnlyTitleRequestDto)
    {
        ValidatePostForGenerateParametres(createPostOnlyTitleRequestDto);

        string prompt = this.openAiSettings.GetPostPrompt(createPostOnlyTitleRequestDto.Title);
        var response = await this.GenerateTextResponse(prompt);

        var postResponse = JsonConvert.DeserializeObject<OpenAiGenerateResponseDto>(response);

        var createPostDto = this.mapper.Map<CreatePostDto>(postResponse);
        createPostDto.Title = createPostOnlyTitleRequestDto.Title;

        return createPostDto;
    }

    private static void ValidatePostForGenerateParametres(CreatePostOnlyTitleRequestDto createPostOnlyTitleRequest)
    {
        ArgumentNullException.ThrowIfNull(createPostOnlyTitleRequest);
    }

    private async Task<string> GenerateTextResponse(string prompt)
    {
        var chatResponse = await this.completionService.CreateCompletionAsync(prompt);
        string response = chatResponse.ToString();
        return response;
    }
}
