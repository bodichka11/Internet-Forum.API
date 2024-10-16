using WebApp.BusinessLogic.DTOs.Topic;

namespace WebApp.BusinessLogic.Services.Interfaces;
public interface ITopicService
{
    Task<TopicDto> CreateTopic(TopicCreateDto topicCreateDto);

    Task DeleteTopic(long id);

    Task<TopicDto> UpdateTopic(TopicDto topicDto);

    Task<IEnumerable<TopicDto>> GetAllTopics(int pageNumber, int pageSize);

    Task<TopicDto> GetTopicById(long id);
}
