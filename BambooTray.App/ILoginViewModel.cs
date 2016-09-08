using System;
using System.Security;
using System.Windows.Input;

namespace BambooTray.App
{
    internal interface ILoginViewModel : IViewModel
    {
        string Username { get; set; }
        SecureString Password { get; set; }
        ICommand LoginCommand { get; set; }
        event EventHandler Close;
    }
}