using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using BambooTray.App.Bamboo.Resources;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App.View.Popup
{
    public class PopupViewModelDesignData : IPopupViewModel
    {
        public PopupViewModelDesignData()
        {
            BambooPlans = new List<BambooPlan>
                {
                    new BambooPlan
                        {
                            BuildName = "Building Build",
                            ProjectName = "Project1",
                            IsBuilding = true,
                            RemainingTime = "15 min remaining",
                            IsEnabled = true
                        },
                    new BambooPlan
                        {
                            BuildState = BuildState.Successful,
                            BuildName = "Successful Build",
                            ProjectName = "Project1",
                            IsEnabled = true
                        },
                    new BambooPlan
                        {
                            BuildState = BuildState.Failed,
                            BuildName = "Failed Build",
                            ProjectName = "Project2",
                            IsEnabled = true
                        },
                    new BambooPlan
                        {
                            BuildState = BuildState.Unknown,
                            BuildName = "Unknown Build",
                            ProjectName = "Project2",
                            IsEnabled = true
                        },
                    new BambooPlan
                        {
                            BuildState = BuildState.Unknown,
                            BuildName = "Disabled Plan",
                            ProjectName = "Project2",
                            IsEnabled = false
                        }
                };
        }

        public Uri IconSource { get; set; }
        public ICommand OpenInBrowserCommand { get; set; }
        public ICollection<BambooPlan> BambooPlans { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<PlanEventArgs> BambooPlanChanged;

        public void PlanChanged(object sender, PlanEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}