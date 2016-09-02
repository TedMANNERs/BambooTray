using System.Collections.ObjectModel;
using System.Linq;
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

        public ObservableCollection<BambooPlan> BambooPlans { get; set; }

        [EventSubscription("topic://BambooTray/PlanChanged", typeof(OnPublisher))]
        public void PlanChanged(object sender, PlanEventArgs e)
        {
            int index = BambooPlans.IndexOf(BambooPlans.FirstOrDefault(x => x.PlanKey == e.Plan.PlanKey));
            if (index != -1)
                BambooPlans[index] = e.Plan;
        }
    }
}