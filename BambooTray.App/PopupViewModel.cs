using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using BambooTray.App.Configuration;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;
using BambooTray.App.SessionManagement;

namespace BambooTray.App
{
    public class PopupViewModel : IPopupViewModel
    {
        private readonly Configuration.Configuration _config;
        private readonly ISessionManager _sessionManager;

        public PopupViewModel(IConfigurationManager configurationManager, ISessionManager sessionManager)
        {
            _config = configurationManager.Config;
            _sessionManager = sessionManager;
            OpenInBrowserCommand = new DelegateCommand(OpenInBrowser, () => true);
        }

        public ICommand OpenInBrowserCommand { get; set; }
        public ObservableCollection<BambooPlan> BambooPlans { get; set; } = new ObservableCollection<BambooPlan>();

        public void Load()
        {
            _sessionManager.OpenSession();
        }

        [EventSubscription(Topics.PlanChanged, typeof(OnPublisher))]
        public void PlanChanged(object sender, PlanEventArgs e)
        {
            int index = BambooPlans.IndexOf(BambooPlans.FirstOrDefault(x => x.PlanKey == e.Plan.PlanKey));
            if (index != -1)
                BambooPlans[index] = e.Plan;
            else
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => BambooPlans.Add(e.Plan)));
        }

        public void Close()
        {
            _sessionManager.CloseSession();
        }

        private void OpenInBrowser(object parameter)
        {
            Process.Start($"{_config.BambooHostname}/browse/{(string)parameter}");
        }
    }
}