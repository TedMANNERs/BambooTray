using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BambooTray.App.Bamboo.Resources;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App
{
    public class PopupViewModelDesignData : IPopupViewModel
    {
        public PopupViewModelDesignData()
        {
            BambooPlans = new ObservableCollection<BambooPlan>
                {
                    new BambooPlan {BuildState = BuildState.Successful, BuildName = "Build", ProjectName = "Project"}
                };
        }

        public Uri IconSource { get; set; }
        public ICommand OpenInBrowserCommand { get; set; }
        public ObservableCollection<BambooPlan> BambooPlans { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<PlanEventArgs> BambooPlanChanged;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void PlanChanged(object sender, PlanEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}