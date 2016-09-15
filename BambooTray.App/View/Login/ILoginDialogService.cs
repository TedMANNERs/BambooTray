using BambooTray.App.Model;

namespace BambooTray.App.View.Login
{
    public interface ILoginDialogService
    {
        LoginCredentials ShowDialog();
    }
}