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
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string search = null)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            try
            {
                var stories = await _service.GetNewestStoriesAsync(pageNumber, pageSize, search);
                return Ok(stories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch stories");
                return StatusCode(500, "Something went wrong");
            }
        }
    }
}