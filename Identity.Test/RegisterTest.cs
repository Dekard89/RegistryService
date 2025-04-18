using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;
using Spotify.Identity.Mediatr.Handlers;

namespace Identity.Test;

public class RegisterTest
{
    private  readonly Mock<UserManager<UserEntity>> userManager;
    
    private readonly Mock<IMapper> mapper;
    
    private readonly Mock<ILogger<RegisterCommandHandler>> logger;

    public RegisterTest()
    {
        var userStore = new Mock<IUserStore<UserEntity>>();
        userManager =
            new Mock<UserManager<UserEntity>>(userStore.Object, null, null, null, null, null, null, null, null);
        mapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Register_Test_Positive()
    {
        //Arrange
        var command= new RegisterCommand{UserName = "Test",
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
        };
        var user = new UserEntity { Id = "user-id", Email = "test@example.com" };
        var identityResult = IdentityResult.Success;
        userManager.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), command.Password))
            .ReturnsAsync(identityResult);
        var handler= new RegisterCommandHandler(userManager.Object,mapper.Object,logger.Object);
        
        //Act
        var result=await handler.Handle(command, CancellationToken.None);
        
        //Assert
        Assert.True(result);

    }

    [Fact]
    public async Task Register_Test_Negative()
    {
        //Arrange
        var command= new RegisterCommand{UserName = "Test",
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
        };
        var user = new UserEntity { Id = "user-id", Email = "test@example.com" };
        var identityResult = IdentityResult.Failed(new IdentityError {Description = "User not found"});
        userManager.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), command.Password))
            .ReturnsAsync(identityResult);
        var handler= new RegisterCommandHandler(userManager.Object,mapper.Object,logger.Object);
        
        //Act
        var result=await handler.Handle(command, CancellationToken.None);
        
        //Assert
        Assert.False(result);
    }
}