using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.BusinessLogic.DTOs.Topic;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.WebApi.Controllers;

/// <summary>
/// Controller for handling topic-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TopicController : ControllerBase
{
    private readonly ITopicService topicService;
    private readonly IMapper mapper;
    private readonly ILogger<TopicController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TopicController"/> class.
    /// </summary>
    /// <param name="topicService">The topic service.</param>
    /// <param name="mapper">The object mapper.</param>
    /// <param name="logger">The logger instance.</param>
    public TopicController(ITopicService topicService, IMapper mapper, ILogger<TopicController> logger)
    {
        this.topicService = topicService;
        this.mapper = mapper;
        this.logger = logger;
    }

    /// <summary>
    /// Gets all topics with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of topics.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllTopics([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return this.BadRequest("Invalid pagination parameters");
        }

        this.logger.LogInformation("Getting all topics with pagination (Page: {PageNumber}, Size: {PageSize})", pageNumber, pageSize);

        var topics = await this.topicService.GetAllTopics(pageNumber, pageSize);
        return this.Ok(topics);
    }

    /// <summary>
    /// Gets a topic by its ID.
    /// </summary>
    /// <param name="id">The topic ID.</param>
    /// <returns>The topic if found.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopicById(long id)
    {
        this.logger.LogInformation("Fetching topic by id {TopicId}", id);

        var topic = await this.topicService.GetTopicById(id);

        if (topic == null)
        {
            this.logger.LogWarning("Topic not found with id {TopicId}", id);
            return this.NotFound();
        }

        return this.Ok(topic);
    }

    /// <summary>
    /// Creates a new topic.
    /// </summary>
    /// <param name="topicCreateDto">The DTO for creating the topic.</param>
    /// <returns>The created topic.</returns>
    [HttpPost]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<TopicDto>> CreateTopic(TopicCreateDto topicCreateDto)
    {
        if (topicCreateDto == null)
        {
            this.logger.LogWarning("Createtopic called with null DTO");
            return this.BadRequest();
        }

        this.logger.LogInformation("Creating new topic");
        var createdTopic = await this.topicService.CreateTopic(topicCreateDto);
        return this.CreatedAtAction(nameof(this.GetTopicById), new { id = createdTopic.Id }, createdTopic);
    }

    /// <summary>
    /// Updates an existing topic.
    /// </summary>
    /// <param name="id">The topic ID.</param>
    /// <param name="topicUpdateDto">The updated topic DTO.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> UpdateTopic(long id, TopicUpdateDto topicUpdateDto)
    {
        if (topicUpdateDto == null)
        {
            this.logger.LogWarning("UpdateTopic called with null DTO");
            return this.BadRequest();
        }

        this.logger.LogInformation("Updating topic with id {TopicId}", id);

        try
        {
            var existingTopic = await this.topicService.GetTopicById(id);

            if (existingTopic == null)
            {
                this.logger.LogWarning("Topic not found with id {TopicId}", id);
                return this.NotFound();
            }

            _ = this.mapper.Map(topicUpdateDto, existingTopic);
            _ = await this.topicService.UpdateTopic(existingTopic);
            return this.NoContent();
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Argument error occurred while updating topic with id {TopicId}", id);
            return this.BadRequest("Invalid arguments provided.");
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Topic not found with id {TopicId}", id);
            return this.NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a topic by its ID.
    /// </summary>
    /// <param name="id">The topic ID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> DeleteTopic(long id)
    {
        this.logger.LogInformation("Deleting topic with id {TopicId}", id);

        try
        {
            var topic = await this.topicService.GetTopicById(id);
            if (topic == null)
            {
                this.logger.LogWarning("Topic not found with id {TopicId}", id);
                return this.NotFound();
            }

            await this.topicService.DeleteTopic(id);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Topic not found with id {TopicId}", id);
            return this.NotFound(ex.Message);
        }
    }
}
