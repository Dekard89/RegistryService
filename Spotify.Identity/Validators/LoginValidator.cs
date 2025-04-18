using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Spotify.Identity.Entity;
using Spotify.Identity.Exeptions;
using Spotify.Identity.Mediatr.Commands;

namespace Spotify.Identity.Validators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator(UserManager<UserEntity> userManager)
    {
        RuleFor(l => l.Email).NotEmpty().WithMessage("Email is required")
            .MustAsync(async (l, token) => await userManager.FindByEmailAsync(l) != null)
            .WithMessage("Email not found");
        RuleFor(l=>l.Password).NotEmpty().WithMessage("Password is require")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");
        RuleFor(l => l).MustAsync(async (login, token) =>
        {
            var user = await userManager.FindByEmailAsync(login.Email);
            if (user == null)
                return false;
            return await userManager.CheckPasswordAsync(user, login.Password);

        }).WithMessage("Invalid password");
    }
}