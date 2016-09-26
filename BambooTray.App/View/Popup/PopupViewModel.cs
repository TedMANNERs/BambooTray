using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using BambooTray.App.Bamboo.Resources;
using BambooTray.App.Configuration;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App.View.Popup
{
    public class PopupViewModel : IPopupViewModel, INotifyPropertyChanged
    {
        private readonly IDictionary<string, BambooPlan> _bambooPlans = new ConcurrentDictionary<string, BambooPlan>();
        private readonly Configuration.Configuration _config;
        private Uri _iconSource;

        public PopupViewModel(IConfigurationManager configurationManager)
        {
            _config = configurationManager.Config;
            OpenInBrowserCommand = new DelegateCommand(OpenInBrowser, () => true);
            IconSource = new Uri("pack://application:,,,/Images/bamboo.ico");
        }

        public Uri IconSource
        {
            get { return _iconSource; }
            set
            {
                _iconSource = value;
                OnPropertyChanged(nameof(IconSource));
            }
        }

        public ICommand OpenInBrowserCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICollection<BambooPlan> BambooPlans => _bambooPlans.Values.OrderBy(x => x.ProjectName).ThenBy(x => x.BuildName).ToList();
        public event EventHandler<PlanEventArgs> BambooPlanChanged;

        [EventSubscription(Topics.PlanChanged, typeof(OnPublisher))]
        public void PlanChanged(object sender, PlanEventArgs e)
        {
            if (_bambooPlans.ContainsKey(e.Plan.PlanKey) && _bambooPlans[e.Plan.PlanKey].IsBuilding != e.Plan.IsBuilding)
                BambooPlanChanged?.Invoke(sender, e);

            _bambooPlans[e.Plan.PlanKey] = e.Plan;
            UpdateIcon(e.Plan);
            OnPropertyChanged(nameof(BambooPlans));
        }

        [EventSubscription(Topics.PlanRemoved, typeof(OnPublisher))]
        public void PlanRemoved(object sender, PlanEventArgs e)
        {
            if (!_bambooPlans.ContainsKey(e.Plan.PlanKey))
                return;

            _bambooPlans.Remove(e.Plan.PlanKey);
            OnPropertyChanged(nameof(BambooPlans));
        }

        private void UpdateIcon(BambooPlan plan)
        {
            switch (plan.BuildState)
            {
                case BuildState.Failed:
                    IconSource = new Uri("pack://application:,,,/Images/bamboo-failed.ico");
                    break;
                case BuildState.Successful:
                    if (_bambooPlans.All(x => x.Value.BuildState == BuildState.Successful))
                        IconSource = new Uri("pack://application:,,,/Images/bamboo-successful.ico");
                    break;
                case BuildState.Unknown:
                    if (_bambooPlans.All(x => x.Value.BuildState == BuildState.Unknown))
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

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}