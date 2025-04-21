using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;
using Spotify.Identity.Validators;

namespace Identity.Test;

public class LoginValidatorTest
{
    
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;

    public LoginValidatorTest()
    {
        var userStore = new Mock<IUserStore<UserEntity>>();
        _userManagerMock =
            new Mock<UserManager<UserEntity>>(userStore.Object, null, null, null, null, null, null, null, null);
    }

    [Theory]
    [InlineData("", "notvalidEmail", false, false, 5,false)]
    [InlineData("", "valid@email.com", true, false, 3, false)]
    [InlineData("short", "valid@email.com", true, false, 2, false)]
    [InlineData("ValidPassword123", "valid@email.com", true, false, 1, false)]
    [InlineData("ValidPassword123", "valid@email.com", true, true, 0, true)]
    public async Task ValidateFailed_NotValidatedLogin(string password, string email, bool userFinded, bool check,
        int errorCount, bool expectedResult)
    {
        //Arrange
        var command = new LoginCommand
        {
            Email = email,
            Password = password
        };
        var user = new UserEntity { Id = "user-id", Email = "valid@email.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(userFinded ? user : null);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(check);
        var validator = new LoginValidator(_userManagerMock.Object);

        //Act
        var result = await validator.ValidateAsync(command);

        //Assert
        Assert.Equal(expectedResult, result.IsValid);
        Assert.Equal(errorCount, result.Errors.Count);
    }

    [Fact]
    public async Task ValidateSucceed_ValidatedLogin()
    {
        //Arrange
        var command = new LoginCommand
        {
            Email = "test@test.com",
            Password = "ValidPassword123"
        };
        var user = new UserEntity { Id = "user-id", Email = "valid@email.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        var validator = new LoginValidator(_userManagerMock.Object);
        
        //Act
        var result = await validator.ValidateAsync(command);
        
        //Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateFail_ThrowExeption()
    {
        //Arange
        var invalidCommand = new LoginCommand
        {
            Email = "test.com",
            Password = "invalidPassword"
        };
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(null as UserEntity);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        var validator = new LoginValidator(_userManagerMock.Object);
        //Act and Assert
        await Assert.ThrowsAsync<ValidationException>(() => validator.ValidateAndThrowAsync(invalidCommand));
    }
}