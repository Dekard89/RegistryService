using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;
using Spotify.Identity.Mediatr.Handlers;

namespace Identity.Test;

public class LoginTest
{
    private  readonly Mock<UserManager<UserEntity>> userManager;
    private readonly Mock<IConfiguration> configuration;

    public LoginTest()
    {
        var userStore = new Mock<IUserStore<UserEntity>>();
        userManager =
            new Mock<UserManager<UserEntity>>(userStore.Object, null, null, null, null, null, null, null, null);
        configuration = new();
    }
    [Fact]
    public async Task Positive_LoginTest()
    {
        
        // Arrange
        var command = new LoginCommand{Email = "test@test.com", Password = "password"};
        var user = new UserEntity { Id = "user-id", Email = "test@example.com" };
        var claims = new List<Claim>();
        var tokenExpiration = 12;
        
        userManager.Setup(x=>x.FindByEmailAsync(It.Is<string>(x=>x == command.Email)))
            .ReturnsAsync(user);
        userManager.Setup(x=>x.GetClaimsAsync(user))
            .ReturnsAsync(claims);
        configuration.Setup(x=>x["Jwt:Issuer"]).Returns("auth-service" );
        configuration.Setup(x => x["Jwt:Expire"]).Returns(tokenExpiration.ToString());
        configuration.Setup(x => x["Jwt:SecretKey"]).Returns("SuperPuperSecretKeyThatIsAtLeast32CharactersLong!");
        var handler= new LoginCommandHandler(userManager.Object, configuration.Object);
        
        // Act
        
        var result= await handler.Handle(command,CancellationToken.None);
        
        
        // Assert
        
        Assert.NotNull(result);
        
    }
    [Fact]
    public async Task Fail_LoginTest_WithEmptyEmail()
    {
        // Arange
        var command = new LoginCommand{Email = "", Password = "password"};
        var user = new UserEntity { Id = "user-id", Email = "test@example.com" };
        var claims = new List<Claim>();
        var tokenExpiration = 12;
        
        userManager.Setup(x=>x.FindByEmailAsync(It.Is<string>(x=>x == command.Email)))
            .ReturnsAsync(user);
        userManager.Setup(x=>x.GetClaimsAsync(user))
            .ReturnsAsync(claims);
        configuration.Setup(x=>x["Jwt:Issuer"]).Returns("auth-service" );
        configuration.Setup(x => x["Jwt:Expire"]).Returns(tokenExpiration.ToString());
        configuration.Setup(x => x["Jwt:SecretKey"]).Returns("SuperPuperSecretKeyThatIsAtLeast32CharactersLong!");
        var handler= new LoginCommandHandler(userManager.Object, configuration.Object);
        
        // Act 
        var result=await handler.Handle(command,CancellationToken.None);
        
        // Assert
        
        userManager.Verify(x=>x.FindByEmailAsync(It.Is<string>(x=>x == String.Empty)), Times.Once());
    }

    [Fact]
    public async Task Fail_LoginTest_WithUserNotFound()
    {
        // Arange
        var command = new LoginCommand{Email = "", Password = "password"};
        var user = new UserEntity { Id = "user-id", Email = "test@example.com" };
        var claims = new List<Claim>();
        var tokenExpiration = 12;
        
        userManager.Setup(x=>x.FindByEmailAsync(It.Is<string>(x=>x == command.Email)))
            .ReturnsAsync((UserEntity)null!);
        userManager.Setup(x=>x.GetClaimsAsync(user))
            .ReturnsAsync(claims);
        configuration.Setup(x=>x["Jwt:Issuer"]).Returns("auth-service" );
        configuration.Setup(x => x["Jwt:Expire"]).Returns(tokenExpiration.ToString());
        configuration.Setup(x => x["Jwt:SecretKey"]).Returns("SuperPuperSecretKeyThatIsAtLeast32CharactersLong!");
        var handler= new LoginCommandHandler(userManager.Object, configuration.Object);
        
        // Act and Assert
       await Assert.ThrowsAsync<NullReferenceException>(()=>handler.Handle(command,CancellationToken.None));
    }
}