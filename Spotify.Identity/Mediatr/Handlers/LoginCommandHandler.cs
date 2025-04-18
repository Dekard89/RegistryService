using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;

namespace Spotify.Identity.Mediatr.Handlers;

public class LoginCommandHandler(UserManager<UserEntity> userManager, IConfiguration configuration) : IRequestHandler<LoginCommand, string>
{
    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user= await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new NullReferenceException("User not found");
        }
                var claims = await userManager.GetClaimsAsync(user);
                claims.Add(new Claim("Id", user.Id));
                var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddHours(int.Parse(s: configuration["Jwt:Expire"] ?? throw new InvalidOperationException())),
                    claims: claims,
                    signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException()))
                        , SecurityAlgorithms.HmacSha256)
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                
                return tokenString;
            
        
    }
}