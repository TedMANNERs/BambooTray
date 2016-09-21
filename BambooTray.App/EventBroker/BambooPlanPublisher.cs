using System;
using Appccelerate.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App.EventBroker
{
    public class BambooPlanPublisher : IBambooPlanPublisher
    {
        public void FirePlanChanged(BambooPlan plan)
        {
            PlanChanged?.Invoke(this, new PlanEventArgs(plan));
        }

        public void FirePlanRemoved(BambooPlan plan)
        {
            PlanRemoved?.Invoke(this, new PlanEventArgs(plan));
        }

        [EventPublication(Topics.PlanChanged)]
        public event EventHandler<PlanEventArgs> PlanChanged;

        [EventPublication(Topics.PlanRemoved)]
        public event EventHandler<PlanEventArgs> PlanRemoved;
    }
}