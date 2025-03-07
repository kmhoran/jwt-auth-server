using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using JWTAuthServer.Controllers;
using JWTAuthServer.Interfaces;
using JWTAuthServer.Models;
using Microsoft.AspNetCore.Http;

namespace JWTAuthServer.Tests.Controllers
{
public class AuthControllerTests
{
    private readonly Mock<IUserAuthentication> _mockAuth;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuth = new Mock<IUserAuthentication>();
        _controller = new AuthController(_mockAuth.Object);
    }

    [Fact]
    public void Login_ReturnsOK_WhenAuthenticationPasses()
    {
        Session session = new Session
        {
            Username = "admin",
        };
        _mockAuth.Setup(repo => repo.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(session);
        var result = _controller.Login(new UserLogin { Username = "admin", Password = "password" });

        var notFoundResult = Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsUnauthorized_WhenAuthenticationThrows()
    {
        _mockAuth.Setup(repo => repo.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentException("Some error"));
        var result = _controller.Login(new UserLogin { Username = "admin", Password = "password" });

        var notFoundResult = Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void Login_Returns500_WhenSomeOtherErrorOccurs()
    {
        _mockAuth.Setup(repo => repo.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Some error"));
        var result = _controller.Login(new UserLogin { Username = "admin", Password = "password" });

        var notFoundResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, notFoundResult.StatusCode);
    }
}
}