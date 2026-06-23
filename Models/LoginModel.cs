namespace LogisticaApp.Models
{
    public class LoginModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";
        public string PasswordConfirm { get; set; } = "";
        public UserRole Role { get; set; } = UserRole.Client;
    }
}
