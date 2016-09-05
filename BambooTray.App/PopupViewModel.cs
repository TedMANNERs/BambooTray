using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using BambooTray.App.Bamboo;
using BambooTray.App.Configuration;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App
{
    public class PopupViewModel : IPopupViewModel
    {
        private readonly Configuration.Configuration _config;

        public PopupViewModel(IBambooService bambooService, IConfigurationManager configurationManager)
        {
            _config = configurationManager.Config;
            OpenInBrowserCommand = new DelegateCommand(OpenInBrowser, () => true);
            bambooService.Start();
        }

        private void OpenInBrowser(object parameter)
        {
            Process.Start($"{_config.BambooHostname}/browse/{(string)parameter}");
        }

        public ObservableCollection<BambooPlan> BambooPlans { get; set; } = new ObservableCollection<BambooPlan>();
        public ICommand OpenInBrowserCommand { get; set; }

        [EventSubscription(Topics.PlanChanged, typeof(OnPublisher))]
        public void PlanChanged(object sender, PlanEventArgs e)
        {
            int index = BambooPlans.IndexOf(BambooPlans.FirstOrDefault(x => x.PlanKey == e.Plan.PlanKey));
            if (index != -1)
                BambooPlans[index] = e.Plan;
            else
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => BambooPlans.Add(e.Plan)));
        }
    }
}