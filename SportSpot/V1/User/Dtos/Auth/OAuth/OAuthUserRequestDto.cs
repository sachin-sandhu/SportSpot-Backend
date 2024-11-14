using SportSpot.V1.User.OAuth;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SportSpot.V1.User.Dtos.Auth.OAuth
{
    public record OAuthUserRequestDto
    {
        [Required]
        public required string AccessToken { get; init; }
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required OAuthProviderType Provider { get; init; }
    }
}
