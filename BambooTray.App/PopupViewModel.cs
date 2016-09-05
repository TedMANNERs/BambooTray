using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using BambooTray.App.Bamboo;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App
{
    public class PopupViewModel : IPopupViewModel
    {
        public PopupViewModel()
        {
            IBambooService bambooService = AppKernel.Get<IBambooService>();
            bambooService.Start();
        }

        public ObservableCollection<BambooPlan> BambooPlans { get; set; } = new ObservableCollection<BambooPlan>();

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