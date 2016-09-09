using BambooTray.App.Model;

namespace BambooTray.App
{
    public interface ILoginDialogService
    {
        LoginCredentials ShowDialog();
    }
}