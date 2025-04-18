using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;

namespace Spotify.Identity.Validators;

public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordValidator(UserManager<UserEntity> userManager)
    {
        RuleFor(u=>u.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Wrong format")
            .MustAsync(async (email,token)=> await userManager.FindByEmailAsync(email)!=null)
            .WithMessage("User not found");
        RuleFor(u => u.OldPassword).NotEmpty().WithMessage("password is required");
        RuleFor(u => u).MustAsync(async (command, token) =>
        {
            var user = await userManager.FindByEmailAsync(command.Email);
            return await userManager.CheckPasswordAsync(user, command.OldPassword);
        });
        RuleFor(u => u.NewPassword).NotEmpty().WithMessage("new password is required")
            .MinimumLength(8).WithMessage("New password must be between 8 and 20 characters")
            .MaximumLength(20).WithMessage("Password must be between 8 and 20 characters")
            .Matches("[A-Z]").WithMessage("Password must contains at least one upper case letter")
            .Matches("[a-z]").WithMessage("Password must contains at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contains numbers");
        RuleFor(r=>r.ConfirmPassword).Equal(r=>r.NewPassword).WithMessage("Passwords do not match");

    }
}