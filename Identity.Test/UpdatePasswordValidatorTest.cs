using System.Runtime.InteropServices;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;
using Spotify.Identity.Validators;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Identity.Test;

public class UpdatePasswordValidatorTest
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;

    public UpdatePasswordValidatorTest()
    {
        var userStore = new Mock<IUserStore<UserEntity>>();
        _userManagerMock =
            new Mock<UserManager<UserEntity>>(userStore.Object, null, null, null, null, null, null, null, null);
    }

    [Theory]
    [InlineData("invalidMail", "", false, false, "invalidpassword", "notMatch",
        false, 6)]
    [InlineData("valid@mail.com", "invalidpassword", true, false, "", "notmatch",
        false, 5)]
    [InlineData("valid@mail.com", "ValidPassword123", true, false, "invalidpassword", "notmatch",
        false, 3)]
    [InlineData("valid@mail.com", "ValidPassword123", true, false, "invalidpassword", "invalidpassword",
        false, 2)]
    [InlineData("valid@mail.com", "ValidPassword123", true, true, "ValidPassword321", "ValidPassword321",
        true, 0)]
    public async Task ValidateFailed_InvalidRequest(string email, string oldPassword, bool userFinded,
        bool oldPasswordVerified,
        string newPassword, string newPasswordConfirm, bool result, int errorCount)
    {
        //Arange
        var command = new UpdatePasswordCommand
        {
            Email = email,
            OldPassword = oldPassword,
            NewPassword = newPassword,
            ConfirmPassword = newPasswordConfirm,
        };
        var user = new UserEntity { Id = "user-id", Email = "valid@email.com" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(userFinded ? user : null);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(oldPasswordVerified);
        var validator = new UpdatePasswordValidator(_userManagerMock.Object);

        //Act
        var resultValidation = await validator.ValidateAsync(command);

        //Assert
        Assert.Equal(result, resultValidation.IsValid);
        Assert.Equal(errorCount, resultValidation.Errors.Count);
    }

    [Fact]
    public async Task ValidateSucceed_Test()
    {
        //Arrange
        //Arrange
        var command = new UpdatePasswordCommand
        {
            Email = "test@test.com",
            OldPassword = "ValidPassword123",
            NewPassword = "ValidPassword321",
            ConfirmPassword = "ValidPassword321",
        };
        var user = new UserEntity { Id = "user-id", Email = "valid@email.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        var validator = new UpdatePasswordValidator(_userManagerMock.Object);

        //Act
        var result = await validator.ValidateAsync(command);

        //Assert
        Assert.True(result.IsValid);

    }
    [Fact]
    public async Task ValidateFail_ThrowExeption()
    {
        //Arange
        var invalidCommand = new UpdatePasswordCommand
        {
            Email = "test.com",
            OldPassword = "invalidPassword",
            NewPassword = "invalidPassword!",
            ConfirmPassword = "invalidPassword",
        };
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(null as UserEntity);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        var validator = new UpdatePasswordValidator(_userManagerMock.Object);
        //Act and Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
            validator.ValidateAndThrowAsync(invalidCommand));
    }
}