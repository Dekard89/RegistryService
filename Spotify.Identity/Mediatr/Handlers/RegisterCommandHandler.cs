using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using Spotify.Identity.Entity;
using Spotify.Identity.Exeptions;
using Spotify.Identity.Mediatr.Commands;
using Spotify.Identity.Profiles;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Spotify.Identity.Mediatr.Handlers;

public class RegisterCommandHandler(UserManager<UserEntity> userManager, IMapper mapper, ILogger<RegisterCommandHandler> logger) 
    : IRequestHandler<RegisterCommand, bool>
{
    
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        
        var user= mapper.Map<UserEntity>(request);
        var result =await userManager.CreateAsync(user, request.Password);
        logger.LogInformation("User created a new account with password.");
        if (!result.Succeeded)
        {
            var problemDetails = new ValidationProblemDetails(
                result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray()))
            {
                Title = "User creation failed",
                Status = StatusCodes.Status400BadRequest
            };
            logger.LogError($"Failed to create account with password.{problemDetails}");
            throw new FailedCreatedException(problemDetails);
        }
        
        await userManager.AddClaimsAsync(user, new List<Claim>()
        {
            new Claim(ClaimTypes.Role, "User"),
            new Claim("IsBanned", "False"),
        });
        logger.LogInformation($"Claims added to user {request.UserName}");
        return result.Succeeded;
    }
}