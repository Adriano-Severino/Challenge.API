using AutoFixture;
using AutoFixture.AutoMoq;
using Coding.Challenge.API.Controllers;
using Coding.Challenge.Dependencies.Managers;
using Coding.Challenge.Dependencies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Coding.Challenge.API.Test
{
    [TestClass]
    public class ContentControllerTest
    {
        private ContentController _controller;
        private Mock<IContentsManager> _managerMock;
        private Mock<ILogger<ContentController>> _loggerMock;
        private Fixture _fixture;

        [TestInitialize]
        public void Initialize()
        {
            _managerMock = new Mock<IContentsManager>();
            _loggerMock = new Mock<ILogger<ContentController>>();
            _controller = new ContentController(_managerMock.Object, _loggerMock.Object);
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public async Task GetManyContents_ReturnsOkResult_WhenContentsExist()
        {
            var content = _fixture.Freeze<IEnumerable<Content>>();

            // Arrange
            var contents = new List<ContentDto> { };
            _managerMock.Setup(x => x.GetManyContents()).ReturnsAsync(content);
            var controller = new ContentController(_managerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.GetManyContents();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetManyContents_NotFound_WhenContentsNotExist()
        {
            var content = new List<Content> { };

            // Arrange
            _managerMock.Setup(x => x.GetManyContents()).ReturnsAsync(content);
            var controller = new ContentController(_managerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.GetManyContents();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetContent_ReturnsOkResult_WhenContentExists()
        {
            var content = _fixture.Freeze<IEnumerable<Content>>();

            // Arrange
            _managerMock.Setup(x => x.GetManyContents()).ReturnsAsync(content);
            var controller = new ContentController(_managerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.GetManyContents();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetContent_NotFound_WhenContentNotExists()
        {
            var content = new List<Content> { };

            // Arrange
            _managerMock.Setup(x => x.GetManyContents()).ReturnsAsync(content);
            var controller = new ContentController(_managerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.GetManyContents();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

    }
}
