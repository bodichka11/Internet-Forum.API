using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApp.BusinessLogic.DTOs.Topic;
using WebApp.BusinessLogic.Helpers;
using WebApp.BusinessLogic.Services.Interfaces;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.BusinessLogic.Services;
public class TopicService : ITopicService
{
    private readonly ITopicRepository topicRepository;
    private readonly IMapper mapper;
    private readonly ILogger<TopicService> logger;

    public TopicService(ITopicRepository topicRepository, IMapper mapper, ILogger<TopicService> logger)
    {
        this.topicRepository = topicRepository;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<TopicDto> CreateTopic(TopicCreateDto topicCreateDto)
    {
        var topic = this.mapper.Map<Topic>(topicCreateDto);
        await this.topicRepository.CreateTopicAsync(topic);
        await this.topicRepository.SaveChangesAsync();

        TopicServiceLogging.TopicCreated(this.logger, topic.Name);
        return this.mapper.Map<TopicDto>(topic);
    }

    public async Task DeleteTopic(long id)
    {
        var topic = await this.topicRepository.GetTopicByIdAsync(id);

        if (topic == null)
        {
            TopicServiceLogging.TopicActionFailed(this.logger, $"Topic with ID {id}");
            throw new KeyNotFoundException("Topic not found");
        }

        await this.topicRepository.DeleteTopicAsync(id);
        await this.topicRepository.SaveChangesAsync();

        TopicServiceLogging.TopicDeleted(this.logger, topic.Name);
    }

    public async Task<TopicDto> UpdateTopic(TopicDto topicDto)
    {
        ValidateTopicParameters(topicDto);

        var topic = await this.topicRepository.GetTopicByIdAsync(topicDto.Id);
        if (topic == null)
        {
            throw new KeyNotFoundException("Topic not found");
        }

        _ = this.mapper.Map(topicDto, topic);

        await this.topicRepository.UpdateTopicAsync(topic);

        return this.mapper.Map<TopicDto>(topic);
    }

    public async Task<IEnumerable<TopicDto>> GetAllTopics(int pageNumber, int pageSize)
    {
        var topics = await this.topicRepository.GetAllTopicAsync(pageNumber, pageSize);
        return this.mapper.Map<IEnumerable<TopicDto>>(topics);
    }

    public async Task<TopicDto> GetTopicById(long id)
    {
        var topic = await this.topicRepository.GetTopicByIdAsync(id);
        return this.mapper.Map<TopicDto>(topic);
    }

    private static void ValidateTopicParameters(TopicDto topicDto)
    {
        ArgumentNullException.ThrowIfNull(topicDto);
    }
}
