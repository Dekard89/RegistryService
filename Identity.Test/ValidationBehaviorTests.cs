using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Spotify.Identity.Mediatr.Pipeline;

namespace Identity.Test;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<string>> _validatorMock;
    
    private readonly ValidationBehavior<string,string> _validationBehaviorMock;

    public ValidationBehaviorTests()
    {
        _validatorMock = new Mock<IValidator<string>>();
        
        _validationBehaviorMock = new ValidationBehavior<string, string>(new [] { _validatorMock.Object });
    }

    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        //Arrange
        var behavior = new ValidationBehavior<string, string>(new  IValidator<string> [] {});
        var request = "test request";
        var response = "response";
        var next= new RequestHandlerDelegate<string>(() => Task.FromResult(response));
        
        //Act
        var result = await behavior.Handle(request, next, CancellationToken.None);
        
        //Assert
        
        Assert.Equal(response, result);
    }

    [Fact]
    public async Task Handle_ValidatorReturnsErrors_ShouldThrowValidationException()
    {
        //Arange
        var request = "test request";
        var validationResult= new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Field", "Error message")
        });
        _validatorMock.Setup(x=>x.ValidateAsync(It.IsAny<ValidationContext<string>>()
        , It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        //Act and Assrt
        var exeption = await Assert.ThrowsAsync<ValidationException>(() => _validationBehaviorMock
            .Handle(request,()=>Task.FromResult("response"),CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidatorNotErrors_ShouldCallNext()
    {
        // Arange
        var request = "test request";
        var response = "response";
        var validationResult = new ValidationResult();
        _validatorMock.Setup(x=>x.ValidateAsync(It.IsAny<ValidationContext<string>>()
        , It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        var next = new RequestHandlerDelegate<string>(() => Task.FromResult(response));
        
        //Act
        var result= await _validationBehaviorMock.Handle(request, next, CancellationToken.None);
        
        //Assert
        Assert.Equal(response, result);
        
    }
}