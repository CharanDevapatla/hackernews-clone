using HackerNewsAPI.DTOs;
using HackerNewsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly IHackerNewsService _service;
        private readonly ILogger<StoriesController> _logger;

        public StoriesController(IHackerNewsService service, ILogger<StoriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("newest")]
        public async Task<ActionResult<PagedResult<StoryDto>>> GetNewest(
            [FromQuery] int page = 1,
            [FromQuery] int size = 20,
            [FromQuery] string search = null)
        {
            page = Math.Max(1, page);
            size = Math.Clamp(size, 1, 100);

            try
            {
                var stories = await _service.GetNewestStoriesAsync(page, size, search);
                return stories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch stories");
                return Problem("Something went wrong");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StoryDto>> GetById(int id)
        {
            try
            {
                var story = await _service.GetStoryByIdAsync(id);
                return story != null ? story : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch story {Id}", id);
                return Problem("Something went wrong");
            }
        }
    }
}