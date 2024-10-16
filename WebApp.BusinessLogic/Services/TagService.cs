using WebApp.BusinessLogic.Services.Interfaces;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.BusinessLogic.Services;
public class TagService : ITagService
{
    private readonly ITagRepository tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        this.tagRepository = tagRepository;
    }

    public async Task AddTagsToPostAsync(long postId, ICollection<string> tagNames)
    {
        ValidateTagParameters(tagNames);

        foreach (var tagName in tagNames)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                var tag = await this.tagRepository.GetTagByNameAsync(tagName);

                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                }

                await this.tagRepository.AddTagToPostAsync(postId, tag);
            }
        }
    }

    private static void ValidateTagParameters(ICollection<string> tagNames)
    {
        ArgumentNullException.ThrowIfNull(tagNames);
    }
}
