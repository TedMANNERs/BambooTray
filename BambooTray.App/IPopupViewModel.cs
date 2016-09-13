using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App
{
    public interface IPopupViewModel : IViewModel
    {
        Uri IconSource { get; set; }
        ICommand OpenInBrowserCommand { get; set; }
        ObservableCollection<BambooPlan> BambooPlans { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
        event EventHandler<PlanEventArgs> BambooPlanChanged;

        void PlanChanged(object sender, PlanEventArgs e);
    }
}