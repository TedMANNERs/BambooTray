using System.Windows.Input;
using BambooTray.App.View.Popup;

namespace BambooTray.App.View
{
    public interface ITrayIconViewModel : IViewModel
    {
        ICommand LoginCommand { get; set; }
        ICommand ExitCommand { get; set; }
        IPopupViewModel PopupViewModel { get; set; }

        void Load();

        void Close();
    }
}