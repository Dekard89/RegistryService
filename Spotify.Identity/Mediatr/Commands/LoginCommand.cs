using System.Runtime.Serialization;
using MediatR;

namespace Spotify.Identity.Mediatr.Commands;

[DataContract]
public record LoginCommand : IRequest<string>
{
    [DataMember]
    public required string Email { get; set; }
    
    [DataMember]
    public required string Password { get; set; }
}