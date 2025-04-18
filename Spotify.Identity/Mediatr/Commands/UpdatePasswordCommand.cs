using System.Runtime.Serialization;
using MediatR;

namespace Spotify.Identity.Mediatr.Commands;
[DataContract]
public record UpdatePasswordCommand : IRequest<bool>
{
    [DataMember]
    public required string Email{ get; set; }
    
    [DataMember]
    public required string OldPassword{ get; set; }
    
    [DataMember]
    public required string NewPassword{ get; set; }
    
    [DataMember]
    public required string ConfirmPassword{ get; set; }
}