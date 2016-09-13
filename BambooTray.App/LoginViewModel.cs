using System;
using System.Security;

namespace BambooTray.App
{
    public class LoginViewModel : ILoginViewModel
    {
        public LoginViewModel()
        {
            Username = Environment.UserName;
        }

        public string Username { get; set; }
        public SecureString Password { get; set; } = new SecureString();
    }
}