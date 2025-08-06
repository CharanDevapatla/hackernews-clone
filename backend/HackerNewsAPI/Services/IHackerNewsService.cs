using HackerNewsAPI.DTOs;

namespace HackerNewsAPI.Services
{
    public interface IHackerNewsService
    {
        Task<PagedResult<StoryDto>> GetNewestStoriesAsync(int pageNumber, int pageSize, string search = null);
    }
}