using Microsoft.AspNetCore.Identity;

namespace Spotify.Identity.Entity;

public class UserEntity : IdentityUser
{
    public DateOnly DateOfBirth { get; init; }
}