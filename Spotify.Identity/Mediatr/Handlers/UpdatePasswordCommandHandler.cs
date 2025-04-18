using MediatR;
using Microsoft.AspNetCore.Identity;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;

namespace Spotify.Identity.Mediatr.Handlers;

public class UpdatePasswordCommandHandler(UserManager<UserEntity> userManager) : IRequestHandler<UpdatePasswordCommand, bool>
{
    public async Task<bool> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.OldPassword)) return false;
        var result= await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
           
        return result.Succeeded;
    }
}