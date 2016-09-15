using System.Security;

namespace BambooTray.App.View.Login
{
    public interface ILoginViewModel : IViewModel
    {
        string Username { get; set; }
        SecureString Password { get; set; }
    }
}