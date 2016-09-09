using System.Security;

namespace BambooTray.App
{
    public class LoginViewModel : ILoginViewModel
    {
        public string Username { get; set; }
        public SecureString Password { get; set; } = new SecureString();
    }
}