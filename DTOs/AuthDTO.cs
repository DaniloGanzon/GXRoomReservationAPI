using GXReservationAPI.Constants;

namespace GXReservationAPI.DTOs
{
    public class AuthDTO
    {
    }

    public class LoginModelDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterModelDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = Roles.Employee;
    }
}
