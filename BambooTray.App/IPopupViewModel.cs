using System;
using System.Collections.Generic;
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
        ICollection<BambooPlan> BambooPlans { get; }
        event PropertyChangedEventHandler PropertyChanged;
        event EventHandler<PlanEventArgs> BambooPlanChanged;

        void PlanChanged(object sender, PlanEventArgs e);
    }
}