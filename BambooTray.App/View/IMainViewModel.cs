using BambooTray.App.View.Popup;

namespace BambooTray.App.View
{
    public interface IMainViewModel : IViewModel
    {
        IPopupViewModel PopupViewModel { get; set; }

        void Close();

        void Load();
    }
}