using HackerNewsAPI.DTOs;
using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace HackerNewsAPI.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;
        private readonly ILogger<HackerNewsService> _logger;
        
        private const string API_BASE = "https://hacker-news.firebaseio.com/v0";
        private const string STORIES_KEY = "new_stories";
        private const string STORY_KEY = "story_{0}";
        private static readonly TimeSpan CACHE_TIME = TimeSpan.FromMinutes(5);

        public HackerNewsService(HttpClient http, IMemoryCache cache, ILogger<HackerNewsService> logger)
        {
            _http = http;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedResult<StoryDto>> GetNewestStoriesAsync(int pageNumber, int pageSize, string search = null)
        {
            var ids = await GetStoryIds();
            
            if (!string.IsNullOrEmpty(search))
            {
                var allStoriesCacheKey = "all_stories_cache";
                List<StoryDto> allStories;
                
                if (!_cache.TryGetValue<List<StoryDto>>(allStoriesCacheKey, out allStories))
                {
                    var tasks = ids.Take(200).Select(GetStoryByIdAsync);
                    var results = await Task.WhenAll(tasks);
                    allStories = results.Where(s => s != null).ToList();
                    _cache.Set(allStoriesCacheKey, allStories, TimeSpan.FromMinutes(10));
                }
                
                var term = search.ToLower();
                var filteredStories = allStories.Where(s => 
                    s.Title.ToLower().Contains(term) || 
                    s.Author.ToLower().Contains(term)
                ).ToList();
                
                var total = filteredStories.Count;
                var items = filteredStories.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                
                return new PagedResult<StoryDto>
                {
                    Items = items,
                    TotalCount = total,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            else
            {
                var startIndex = (pageNumber - 1) * pageSize;
                var endIndex = Math.Min(startIndex + pageSize * 2, ids.Count);
                var pageIds = ids.Skip(startIndex).Take(endIndex - startIndex).ToList();
                
                var tasks = pageIds.Select(GetStoryByIdAsync);
                var results = await Task.WhenAll(tasks);
                var stories = results.Where(s => s != null).Take(pageSize).ToList();
                
                return new PagedResult<StoryDto>
                {
                    Items = stories,
                    TotalCount = ids.Count,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        private async Task<StoryDto> GetStoryByIdAsync(int id)
        {
            var key = string.Format(STORY_KEY, id);
            
            if (_cache.TryGetValue<StoryDto>(key, out var cached))
                return cached;

            var response = await _http.GetAsync($"{API_BASE}/item/{id}.json");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var story = JsonSerializer.Deserialize<Story>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (story?.Type != "story")
                return null;

            var dto = new StoryDto
            {
                Id = story.Id,
                Title = story.Title ?? "No title",
                Url = story.Url,
                Author = story.By ?? "Unknown",
                PublishedDate = DateTimeOffset.FromUnixTimeSeconds(story.Time).DateTime,
                Score = story.Score,
                CommentsCount = story.Descendants
            };

            _cache.Set(key, dto, CACHE_TIME);
            return dto;
        }

        private async Task<List<int>> GetStoryIds()
        {
            if (_cache.TryGetValue<List<int>>(STORIES_KEY, out var cached))
                return cached;

            var response = await _http.GetAsync($"{API_BASE}/newstories.json");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var ids = JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();

            _cache.Set(STORIES_KEY, ids, CACHE_TIME);
            return ids;
        }
    }
}