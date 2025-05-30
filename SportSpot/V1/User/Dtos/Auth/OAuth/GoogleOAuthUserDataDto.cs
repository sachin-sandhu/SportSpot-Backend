﻿using System.Text.Json.Serialization;

namespace SportSpot.V1.User.Dtos.Auth.OAuth
{
    public record GoogleOAuthUserDataDto
    {
        public required string Sub { get; init; }
        public required string Name { get; init; }
        [JsonPropertyName("given_name")]
        public required string GivenName { get; init; }
        [JsonPropertyName("family_name")]
        public string? FamilyName { get; init; }
        public string? Picture { get; init; }
        public required string Email { get; init; }
        [JsonPropertyName("email_verified")]
        public required bool EmailVerified { get; init; }
    }
}
