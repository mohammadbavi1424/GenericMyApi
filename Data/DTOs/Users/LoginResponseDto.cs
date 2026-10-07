namespace Data.DTOs.Users
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
        public UserDto User { get; set; }
    }

}
