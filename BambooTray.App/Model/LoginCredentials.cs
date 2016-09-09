using System.Security;

namespace BambooTray.App.Model
{
    public class LoginCredentials
    {
        public string Username { get; set; }
        public SecureString Password { get; set; }
    }
}