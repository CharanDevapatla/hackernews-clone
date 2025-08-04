using HackerNewsAPI.DTOs;

namespace HackerNewsAPI.Services
{
    public interface IHackerNewsService
    {
        Task<PagedResult<StoryDto>> GetNewestStoriesAsync(int page, int size, string search = null);
        Task<StoryDto> GetStoryByIdAsync(int id);
    }
}