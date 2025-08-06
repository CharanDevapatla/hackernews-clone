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


        public void Dispose()
        {
            _httpClient?.Dispose();
            _memoryCache?.Dispose();
        }
    }
}