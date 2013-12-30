namespace Collab.Models.Account
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public bool? IsSuccessful { get; set; }
    }
}