namespace MyntraClone.API.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
