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

            var result = await _controller.GetNewest(1, 20, searchTerm);

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

            var result = await _controller.GetNewest(-1, 20);

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

            var result = await _controller.GetNewest(1, 200);

            _serviceMock.Verify(s => s.GetNewestStoriesAsync(1, 100, null), Times.Once);
        }

        [Fact]
        public async Task GetStory_ExistingStory_ReturnsOkResult()
        {
            var storyId = 123;
            var expectedStory = new StoryDto
            {
                Id = storyId,
                Title = "Test Story",
                Author = "testuser"
            };

            _serviceMock.Setup(s => s.GetStoryByIdAsync(storyId))
                       .ReturnsAsync(expectedStory);

            var result = await _controller.GetById(storyId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualStory = Assert.IsType<StoryDto>(okResult.Value);
            Assert.Equal(storyId, actualStory.Id);
            Assert.Equal("Test Story", actualStory.Title);
        }

        [Fact]
        public async Task GetStory_NonExistentStory_ReturnsNotFound()
        {
            var storyId = 999;

            _serviceMock.Setup(s => s.GetStoryByIdAsync(storyId))
                       .ReturnsAsync((StoryDto)null);

            var result = await _controller.GetById(storyId);

            Assert.IsType<NotFoundResult>(result.Result);
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