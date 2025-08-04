using HackerNewsAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace HackerNewsAPI.Tests.Services
{
    public class HackerNewsServiceTests : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILogger<HackerNewsService>> _loggerMock;
        private readonly HackerNewsService _service;

        public HackerNewsServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _loggerMock = new Mock<ILogger<HackerNewsService>>();
            _service = new HackerNewsService(_httpClient, _memoryCache, _loggerMock.Object);
        }

        [Fact]
        public async Task GetStoryByIdAsync_ValidStory_ReturnsStoryDto()
        {
            var storyId = 123;
            var storyJson = JsonSerializer.Serialize(new
            {
                id = storyId,
                title = "Test Story",
                url = "https://example.com",
                by = "testuser",
                time = 1609459200, // Unix timestamp
                score = 100,
                descendants = 10,
                type = "story"
            });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(storyJson)
                });

            var result = await _service.GetStoryByIdAsync(storyId);

            Assert.NotNull(result);
            Assert.Equal(storyId, result.Id);
            Assert.Equal("Test Story", result.Title);
            Assert.Equal("https://example.com", result.Url);
            Assert.Equal("testuser", result.Author);
            Assert.Equal(100, result.Score);
            Assert.Equal(10, result.CommentsCount);
        }

        [Fact]
        public async Task GetStoryByIdAsync_NonStoryType_ReturnsNull()
        {
            var storyId = 123;
            var commentJson = JsonSerializer.Serialize(new
            {
                id = storyId,
                text = "This is a comment",
                by = "testuser",
                time = 1609459200,
                type = "comment"
            });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(commentJson)
                });

            var result = await _service.GetStoryByIdAsync(storyId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetStoryByIdAsync_CachedStory_ReturnsCachedResult()
        {
            var storyId = 123;
            var cachedStory = new HackerNewsAPI.DTOs.StoryDto
            {
                Id = storyId,
                Title = "Cached Story",
                Author = "cached_user"
            };

            _memoryCache.Set($"story_{storyId}", cachedStory);

            var result = await _service.GetStoryByIdAsync(storyId);

            Assert.NotNull(result);
            Assert.Equal("Cached Story", result.Title);
            Assert.Equal("cached_user", result.Author);

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetStoryByIdAsync_HttpError_ReturnsNull()
        {
            var storyId = 123;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var result = await _service.GetStoryByIdAsync(storyId);

            Assert.Null(result);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _memoryCache?.Dispose();
        }
    }
}