using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using JWTAuthServer.Controllers;
using JWTAuthServer.Interfaces;
using JWTAuthServer.Models;
using Microsoft.AspNetCore.Http;

namespace JWTAuthServer.Tests.Controllers
{
public class UserControllerTests
{
    private readonly Mock<IDataRepo> _mockRepo;
    private readonly Mock<IUserAuthentication> _mockAuth;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockRepo = new Mock<IDataRepo>();
        _mockAuth = new Mock<IUserAuthentication>();
        _controller = new UserController(_mockRepo.Object, _mockAuth.Object);
    }

    [Fact]
    public void GetUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns((User)null);
        var result = _controller.GetUser("nonexistentUsername");

        var notFoundResult = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetUser_ReturnsOk_WhenUserExists()
    {
        var user = new User {
            Id = "1",
            Username = "admin",
            PasswordHash = "passwordHash",
            PasswordSalt = "passwordSalt",
            Created = new DateTime(2021, 1, 1),
            LastActive = DateTime.Now
        };
        _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user); 

        var result = _controller.GetUser("admin");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal("admin", returnedUser.Username);
    }

    [Fact]
    public void Register_ReturnsOk_WhenUserIsAdded()
    {
        var user = new UserRegistrationRequest {
            Username = "newUser",
            Password = "password"
        };

        var result = _controller.Register(user);

        var okResult = Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Register_ReturnsConflict_WhenUserAlreadyExists()
    {
        var user = new User {
            Id = "1",
            Username = "admin",
            PasswordHash = "passwordHash",
            PasswordSalt = "passwordSalt",
            Created = new DateTime(2021, 1, 1),
            LastActive = DateTime.Now
        };
        _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user); 

        var userRegistrationRequest = new UserRegistrationRequest {
            Username = "newUser",
            Password = "password"
        };

        var result = _controller.Register(userRegistrationRequest);

        Assert.IsType<ConflictObjectResult>(result);
    }
}
}