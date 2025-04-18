using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;

namespace Spotify.Identity.Validators;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator(UserManager<UserEntity> userManager)
    {
        RuleFor(r => r.Email).NotEmpty().WithMessage("Email is required")
            .MustAsync(async (email, token) => await userManager.FindByEmailAsync(email) == null)
            .WithMessage("Email is already registered").EmailAddress()
            .WithMessage("Email-format is required");
        RuleFor(r=>r.UserName).NotEmpty().WithMessage("Username is required")
            .MustAsync(async (username,token) => await userManager.FindByNameAsync(username) == null)
            .WithMessage("Username is already registered").MinimumLength(2).WithMessage("Username must be between 2 and 20 characters")
            .MinimumLength(2).WithMessage("Username must be between 2 and 20 characters")
            .MaximumLength(20).WithMessage("Username must be between 2 and 20 characters");
        RuleFor(r => r.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be between 8 and 20 characters")
            .MaximumLength(20).WithMessage("Password must be between 8 and 20 characters")
            .Matches("[A-Z]").WithMessage("Password must contains at least one upper case letter")
            .Matches("[a-z]").WithMessage("Password must contains at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contains numbers");
        RuleFor(r=>r.ConfirmPassword).Equal(r=>r.Password).WithMessage("Passwords do not match");
        RuleFor(r => r.DateOfBirth).NotEmpty().WithMessage("Date of birth is required");
    }


}