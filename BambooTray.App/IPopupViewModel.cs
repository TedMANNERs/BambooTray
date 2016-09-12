using System;
using System.Collections.ObjectModel;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App
{
    public interface IPopupViewModel : IViewModel
    {
        ObservableCollection<BambooPlan> BambooPlans { get; set; }
        event EventHandler<PlanEventArgs> BambooPlanChanged;

        void PlanChanged(object sender, PlanEventArgs e);

        void Close();
    }
}