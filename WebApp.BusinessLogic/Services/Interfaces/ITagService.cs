namespace WebApp.BusinessLogic.Services.Interfaces;
public interface ITagService
{
    Task AddTagsToPostAsync(long postId, ICollection<string> tagNames);
}
