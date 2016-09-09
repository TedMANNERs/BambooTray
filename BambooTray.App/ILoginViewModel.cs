using System.Security;

namespace BambooTray.App
{
    public interface ILoginViewModel : IViewModel
    {
        string Username { get; set; }
        SecureString Password { get; set; }
    }
}