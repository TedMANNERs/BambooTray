using System;
using BambooTray.App.EventBroker;

namespace BambooTray.App
{
    public interface IMainViewModel : IViewModel
    {
        IPopupViewModel PopupViewModel { get; set; }
        event EventHandler<PlanEventArgs> BambooPlanChanged;

        void Close();

        void Load();
    }
}