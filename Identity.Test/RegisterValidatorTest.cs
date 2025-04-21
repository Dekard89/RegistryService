using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;
using Spotify.Identity.Validators;
using Xunit.Sdk;

namespace Identity.Test;

public class RegisterValidatorTest
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;

    public RegisterValidatorTest()
    {
        var userStore = new Mock<IUserStore<UserEntity>>();
        _userManagerMock =
            new Mock<UserManager<UserEntity>>(userStore.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Validate_ShouldBeValidated()
    {
        //Arange
        var command= new RegisterCommand{UserName = "Test",
            Email = "test@test.com",
            Password = "ValidPassword123",
            ConfirmPassword = "ValidPassword123",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
        };
        var validationResult = new ValidationResult();
        
        _userManagerMock.Setup(x=>x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(null as UserEntity);
        _userManagerMock.Setup(x=>x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(null as UserEntity);
        
        var validator= new RegisterValidator(_userManagerMock.Object);
        
        //Act
        
        var result = await validator.ValidateAsync(command);
        
        //Assert
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(false,"invalidpassword","notmatchedpassword","test.com",3)]
    [InlineData(false,"invalidpassword","invalidpassword","test@example.com",1)]
    [InlineData(false,"ValidPassword123","ValidPassword124","test@example.com",1)]
    [InlineData(true,"ValidPassword123","ValidPassword123","test@example.com",0)]
    public async Task Validate_ShouldNotBeValidated_WhereUserIsNull(bool expectedResult,string password,
        string confirmPassword,string email, int errorCount)
    {
        //Arange
        var command= new RegisterCommand{UserName = "Test",
            Email = email,
            Password = password,
            ConfirmPassword = confirmPassword,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
        };
        var validationResult = new ValidationResult();
        
        _userManagerMock.Setup(x =>x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(null as UserEntity);
        _userManagerMock.Setup(x=>x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(null as UserEntity);
        
        var validator= new RegisterValidator(_userManagerMock.Object);
        
        //Act
        
        var result = await validator.ValidateAsync(command);
      
       
        //Assert
        
        Assert.Equal(expectedResult,result.IsValid);
        Assert.Equal(errorCount,result.Errors.Count);

    }

    [Fact]
    public async Task ValidateFailed_ShouldNotBeValidated_WhereUserIsNotNull_()
    {
        //Arange
        var command= new RegisterCommand{UserName = "Test",
            Email = "test@test.com",
            Password = "ValidPassword123",
            ConfirmPassword = "ValidPassword123",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
        };
        var userEntity = new UserEntity{Email = "test@test.com"};
        var userNamefailure = new ValidationFailure("UserName", "Username is already registered");
        var emailfailure = new ValidationFailure("Email", "Email is already registered");
        var validationResult = new ValidationResult(new List<ValidationFailure> {userNamefailure, emailfailure});
        _userManagerMock.Setup(x=>x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(userEntity);
        _userManagerMock.Setup(x=>x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(userEntity);
        var validator= new RegisterValidator(_userManagerMock.Object);
        //Act
        var result = await validator.ValidateAsync(command);
        
        //Asser
        Assert.False(result.IsValid);
        Assert.Equal(2,result.Errors.Count);
        var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.Contains("Username is already registered", errorMessages);
        Assert.Contains("Email is already registered", errorMessages);
    }

    [Fact]
    public async Task ValidateFailed_ShouldNotBeValidated_WhereThrowException_()
    {
        //Arange
        var invalidCommand =new RegisterCommand
        {
            UserName = "Test",
            Email = "test.com",
            Password = "invalid",
            ConfirmPassword = "notmanch",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
        };
        _userManagerMock.Setup(x=>x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(null as UserEntity);
        _userManagerMock.Setup(x=>x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(null as UserEntity);
        var validator= new RegisterValidator(_userManagerMock.Object);
        
        //Act and Assert
        
        await Assert.ThrowsAsync<ValidationException>(() => validator.ValidateAndThrowAsync(invalidCommand));
    }
}