using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BambooTray.App.Bamboo.Resources;
using BambooTray.App.Configuration;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App.Bamboo
{
    public class BambooPlanUpdater : IBambooPlanUpdater
    {
        private readonly IBambooClient _bambooClient;
        private readonly IBambooPlanPublisher _bambooPlanPublisher;
        private readonly Configuration.Configuration _config;
        private EventHandler _onSessionExpired;
        private CancellationTokenSource _source;

        public BambooPlanUpdater(
            IConfigurationManager configurationManager,
            IBambooPlanPublisher bambooPlanPublisher,
            IBambooClient bambooClient)
        {
            _bambooPlanPublisher = bambooPlanPublisher;
            _bambooClient = bambooClient;
            _config = configurationManager.Config;
        }

        public void Start(Session session, EventHandler onSessionExpired)
        {
            _onSessionExpired = onSessionExpired;
            _source = new CancellationTokenSource();
            Task.Run(() => UpdatePlans(session, _source.Token), _source.Token);
        }

        public void Stop()
        {
            _source.Cancel();
        }

        private async Task UpdatePlans(Session session, CancellationToken token)
        {
            Dictionary<string, Result> oldResults = new Dictionary<string, Result>();

            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(5000);
                Plans plans = await GetFavouritePlans(session).ConfigureAwait(false);
                if (plans == null)
                    continue;

                foreach (Plan plan in plans.PlanList)
                {
                    if (plan.IsBuilding && oldResults.Any(x => x.Key == plan.PlanKey.Key))
                        continue;

                    Result newResult = await GetLatestBuild(session, plan.PlanKey.Key).ConfigureAwait(false);
                    if (newResult == null)
                        continue;

                    if (oldResults.Any(x => x.Key == plan.PlanKey.Key) && newResult.Equals(oldResults[plan.PlanKey.Key]))
                        continue;

                    oldResults[plan.PlanKey.Key] = newResult;
                    BambooPlan newBambooPlan = new BambooPlan
                        {
                            BuildName = plan.BuildName,
                            PlanKey = plan.PlanKey.Key,
                            BuildState = newResult.BuildState,
                            ProjectKey = plan.ProjectKey,
                            ProjectName = plan.ProjectName,
                            IsBuilding = plan.IsBuilding
                        };
                    _bambooPlanPublisher.FirePlanChanged(newBambooPlan);
                }
            }
        }

        private Task<Plans> GetFavouritePlans(Session session)
        {
            return GetResource<Plans>($"{_config.BambooHostname}rest/api/latest/plan/?favourite&expand=plans.plan", session);
        }

        private Task<Result> GetLatestBuild(Session session, string planKey)
        {
            return GetResource<Result>($"{_config.BambooHostname}rest/api/latest/result/{planKey}/latest", session);
        }

        private async Task<T> GetResource<T>(string url, Session session) where T : class
        {
            IRestResponse<T> resultResponse = await _bambooClient.GetAsync<T>(url, session.SessionId).ConfigureAwait(false);
            if (resultResponse.IsSuccess)
                return resultResponse.Data;

            if (resultResponse.StatusCode == HttpStatusCode.Unauthorized)
                _onSessionExpired.Invoke(this, EventArgs.Empty);

            return null;
        }
    }
}