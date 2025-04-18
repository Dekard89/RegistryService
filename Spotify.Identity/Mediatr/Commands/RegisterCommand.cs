using System.Runtime.Serialization;
using MediatR;

namespace Spotify.Identity.Mediatr.Commands;

[DataContract]
public record RegisterCommand : IRequest<bool>
{
    [DataMember]
    public required string UserName { get; set; }
    [DataMember]
    public required string Email { get; init; }
    
    [DataMember]
    public required string Password { get; init; }
    
    [DataMember]
    public required string ConfirmPassword { get; init; }
    
    [DataMember]
    public required DateOnly DateOfBirth { get; init; }
}