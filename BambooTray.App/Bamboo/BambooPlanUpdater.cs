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
                    BambooPlan newBambooPlan = new BambooPlan();
                    Result newResult;
                    if (plan.IsBuilding)
                    {
                        newResult = await GetBuildingBuild(session, plan.PlanKey.Key).ConfigureAwait(false);
                        if (newResult != null)
                            newBambooPlan.RemainingTime = newResult.Progress.PrettyTimeRamaining;
                    }
                    else
                        newResult = await GetLatestBuild(session, plan.PlanKey.Key).ConfigureAwait(false);

                    if (newResult == null)
                        continue;

                    if (oldResults.Any(x => x.Key == plan.PlanKey.Key) && newResult.Equals(oldResults[plan.PlanKey.Key]))
                        continue;

                    oldResults[plan.PlanKey.Key] = newResult;
                    newBambooPlan.BuildName = plan.BuildName;
                    newBambooPlan.PlanKey = plan.PlanKey.Key;
                    newBambooPlan.BuildState = newResult.BuildState;
                    newBambooPlan.ProjectKey = plan.ProjectKey;
                    newBambooPlan.ProjectName = plan.ProjectName;
                    newBambooPlan.IsBuilding = plan.IsBuilding;
                    newBambooPlan.IsEnabled = plan.Enabled;
                    _bambooPlanPublisher.FirePlanChanged(newBambooPlan);
                }
            }
        }

        private async Task<Result> GetBuildingBuild(Session session, string planKey)
        {
            Results latestResult = await GetResource<Results>($"{_config.BambooHostname}rest/api/latest/result/{planKey}/?max-results=1&expand=results.result", session).ConfigureAwait(false);
            int buildNumber = latestResult.ResultList.First().BuildNumber;
            return await GetResource<Result>($"{_config.BambooHostname}rest/api/latest/result/{planKey}/{++buildNumber}", session).ConfigureAwait(false);
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