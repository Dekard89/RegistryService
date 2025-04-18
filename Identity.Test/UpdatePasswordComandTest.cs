using Microsoft.AspNetCore.Identity;
using Moq;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;
using Spotify.Identity.Mediatr.Handlers;

namespace Identity.Test;

public class UpdatePasswordComandTest
{
    private  readonly Mock<UserManager<UserEntity>> userManager;

    public UpdatePasswordComandTest()
    {
        var userStore = new Mock<IUserStore<UserEntity>>();
        userManager =
            new Mock<UserManager<UserEntity>>(userStore.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Update_Test_Positive()
    {
        //Arrange
        var command = new UpdatePasswordCommand
        {
            Email = "test@example.com",

            OldPassword = "test",

            NewPassword = "test1",

            ConfirmPassword = "test1"
        };
        var user = new UserEntity { Id = "user-id", Email = "test@example.com" };
        var identityResult = IdentityResult.Success;
        userManager.Setup( x => x.FindByEmailAsync(It.Is<string>(x=>x==command.Email)))
            .ReturnsAsync(user);
        userManager.Setup(x => x.CheckPasswordAsync(user, command.OldPassword)).ReturnsAsync(true);

        userManager.Setup(x => x.ChangePasswordAsync(user,command.OldPassword,command.NewPassword))
            .ReturnsAsync(identityResult);
        //Act
        var handler = new UpdatePasswordCommandHandler(userManager.Object);
        var result=await handler.Handle(command, CancellationToken.None);
        
        //Assert
        userManager.Verify(x => x.FindByEmailAsync(command.Email), Times.Once);
        userManager.Verify(x => x.ChangePasswordAsync(user, command.OldPassword, command.NewPassword), Times.Once);
        Assert.True(result);
    }
    [Fact]
    public async Task Update_Test_Negative()
    {
        //Arrange
        var command = new UpdatePasswordCommand
        {
            Email = "test@example.com",

            OldPassword = "test",

            NewPassword = "test1",

            ConfirmPassword = "test1"
        };
        var user = new UserEntity { Id = "user-id", Email = "test@example.com" };
        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Invalid password" });
        userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        userManager.Setup(x => x.ChangePasswordAsync(It.IsAny<UserEntity>(), It
            .IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        //Act
        var handler = new UpdatePasswordCommandHandler(userManager.Object);
        var result=await handler.Handle(command, CancellationToken.None);
        
        //Assert
        
        userManager.Verify(x=>x.FindByEmailAsync(command.Email), Times.Once());
        Assert.False(result);
    }
}