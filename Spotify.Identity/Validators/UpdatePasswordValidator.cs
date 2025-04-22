using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                return user!=null
                       && await userManager.CheckPasswordAsync(user, command.OldPassword);
            })
            .WithMessage("Invalid password");
        RuleFor(u => u.NewPassword).NotEmpty().WithMessage("new password is required")
            .MinimumLength(8).WithMessage("New password must be between 8 and 20 characters")
            .MaximumLength(20).WithMessage("Password must be between 8 and 20 characters")
            .Matches("^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).*$").WithMessage("Password must contains at least one upper case letter,lowercase letter and number")
            .NotEqual(c => c.OldPassword).WithMessage("Password must not match the old one");
        RuleFor(r=>r.ConfirmPassword).Equal(r=>r.NewPassword).WithMessage("Passwords do not match");

    }
}