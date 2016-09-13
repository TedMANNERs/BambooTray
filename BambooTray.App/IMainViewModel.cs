namespace BambooTray.App
{
    public interface IMainViewModel : IViewModel
    {
        IPopupViewModel PopupViewModel { get; set; }

        void Close();

        void Load();
    }
}