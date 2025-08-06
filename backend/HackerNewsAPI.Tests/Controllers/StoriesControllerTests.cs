using HackerNewsAPI.Controllers;
using HackerNewsAPI.DTOs;
using HackerNewsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HackerNewsAPI.Tests.Controllers
{
    public class StoriesControllerTests
    {
        private readonly Mock<IHackerNewsService> _serviceMock;
        private readonly Mock<ILogger<StoriesController>> _loggerMock;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _serviceMock = new Mock<IHackerNewsService>();
            _loggerMock = new Mock<ILogger<StoriesController>>();
            _controller = new StoriesController(_serviceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetNewestStories_ValidRequest_ReturnsOkResult()
        {
            var expectedResult = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto>
                {
                    new StoryDto { Id = 1, Title = "Story 1", Author = "user1" },
                    new StoryDto { Id = 2, Title = "Story 2", Author = "user2" }
                },
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 20
            };

            _serviceMock.Setup(s => s.GetNewestStoriesAsync(1, 20, null))
                       .ReturnsAsync(expectedResult);

            var result = await _controller.GetNewest();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualResult = Assert.IsType<PagedResult<StoryDto>>(okResult.Value);
            Assert.Equal(2, actualResult.Items.Count);
            Assert.Equal(2, actualResult.TotalCount);
        }

        [Fact]
        public async Task GetNewestStories_WithSearchTerm_ReturnsFilteredResults()
        {
            var searchTerm = "angular";
            var expectedResult = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto>
                {
                    new StoryDto { Id = 1, Title = "Angular Framework", Author = "dev1" }
                },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 20
            };

            _serviceMock.Setup(s => s.GetNewestStoriesAsync(1, 20, searchTerm))
                       .ReturnsAsync(expectedResult);

            var result = await _controller.GetNewest(pageNumber: 1, pageSize: 20, search: searchTerm);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualResult = Assert.IsType<PagedResult<StoryDto>>(okResult.Value);
            Assert.Single(actualResult.Items);
            Assert.Contains("Angular", actualResult.Items.First().Title);
        }

        [Fact]
        public async Task GetNewestStories_InvalidPageNumber_CorrectsToOne()
        {
            var expectedResult = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 20
            };

            _serviceMock.Setup(s => s.GetNewestStoriesAsync(1, 20, null))
                       .ReturnsAsync(expectedResult);

            var result = await _controller.GetNewest(pageNumber: -1, pageSize: 20);

            _serviceMock.Verify(s => s.GetNewestStoriesAsync(1, 20, null), Times.Once);
        }

        [Fact]
        public async Task GetNewestStories_LargePageSize_ClampsToMaximum()
        {
            var expectedResult = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 100
            };

            _serviceMock.Setup(s => s.GetNewestStoriesAsync(1, 100, null))
                       .ReturnsAsync(expectedResult);

            var result = await _controller.GetNewest(pageNumber: 1, pageSize: 200);

            _serviceMock.Verify(s => s.GetNewestStoriesAsync(1, 100, null), Times.Once);
        }


        [Fact]
        public async Task GetNewestStories_ServiceThrowsException_ReturnsInternalServerError()
        {
            _serviceMock.Setup(s => s.GetNewestStoriesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                       .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetNewest();

            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}