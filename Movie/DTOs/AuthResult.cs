using System.Text.Json.Serialization;

namespace Movie.DTOs
{
    public class AuthResult
    {
        public List<string>? Errors {  get; set; }
        public string? Token { get; set; }
        public bool Success {  get; set; }
        public DateTime? ExpiresOn { get; set; }


        // for refreshToken
        [JsonIgnore]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
