using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using BambooTray.App.Bamboo.Resources;
using BambooTray.App.Configuration;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;
using BambooTray.App.SessionManagement;

namespace BambooTray.App
{
    public class PopupViewModel : IPopupViewModel, INotifyPropertyChanged
    {
        private readonly Configuration.Configuration _config;
        private readonly ISessionManager _sessionManager;
        private Uri _iconSource;

        public PopupViewModel(IConfigurationManager configurationManager, ISessionManager sessionManager)
        {
            _config = configurationManager.Config;
            _sessionManager = sessionManager;
            OpenInBrowserCommand = new DelegateCommand(OpenInBrowser, () => true);
            IconSource = new Uri("pack://application:,,,/Images/bamboo.ico");
        }

        public Uri IconSource
        {
            get { return _iconSource; }
            set
            {
                _iconSource = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenInBrowserCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<BambooPlan> BambooPlans { get; set; } = new ObservableCollection<BambooPlan>();
        public event EventHandler<PlanEventArgs> BambooPlanChanged;

        public void Close()
        {
            _sessionManager.CloseSession();
        }

        [EventSubscription(Topics.PlanChanged, typeof(OnPublisher))]
        public void PlanChanged(object sender, PlanEventArgs e)
        {
            int index = BambooPlans.IndexOf(BambooPlans.FirstOrDefault(x => x.PlanKey == e.Plan.PlanKey));
            if (index != -1)
            {
                Application.Current.Dispatcher.Invoke(() => { BambooPlans[index] = e.Plan; });
                BambooPlanChanged?.Invoke(sender, e);
                OnPropertyChanged("BambooPlans");
            }
            else
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => BambooPlans.Add(e.Plan)));

            UpdateIcon(e.Plan);
        }

        public void Load()
        {
            _sessionManager.OpenSession();
        }

        private void UpdateIcon(BambooPlan plan)
        {
            switch (plan.BuildState)
            {
                case BuildState.Failed:
                    IconSource = new Uri("pack://application:,,,/Images/bamboo-failed.ico");
                    break;
                case BuildState.Successful:
                    if (!BambooPlans.Any(x => x.BuildState != BuildState.Successful))
                        IconSource = new Uri("pack://application:,,,/Images/bamboo-successful.ico");
                    break;
                case BuildState.Unknown:
                    if (!BambooPlans.Any(x => x.BuildState == BuildState.Successful || x.BuildState == BuildState.Failed))
                        IconSource = new Uri("pack://application:,,,/Images/bamboo.ico");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OpenInBrowser(object parameter)
        {
            Process.Start($"{_config.BambooHostname}/browse/{(string)parameter}");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}