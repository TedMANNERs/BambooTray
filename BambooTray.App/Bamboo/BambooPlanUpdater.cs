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
        private const int Timeout = 5000;
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
            _source?.Cancel();
        }

        private async Task UpdatePlans(Session session, CancellationToken token)
        {
            Dictionary<string, Result> oldResults = new Dictionary<string, Result>();

            while (!token.IsCancellationRequested)
            {
                Plans plans = await GetFavouritePlans(session).ConfigureAwait(false);
                if (plans != null)
                {
                    IEnumerable<Task<Result>> resultTasks = plans.PlanList.Select(plan => GetLatestBuild(session, plan.PlanKey.Key));
                    Result[] results = await Task.WhenAll(resultTasks);

                    if (plans.PlanList.Count < oldResults.Count)
                    {
                        string removedPlanKey = oldResults.First(x => !plans.PlanList.Any(y => y.PlanKey.Key == x.Key)).Key;
                        oldResults.Remove(removedPlanKey);
                        _bambooPlanPublisher.FirePlanRemoved(new BambooPlan { PlanKey = removedPlanKey });
                    }

                    foreach (PlanResult planResult in plans.PlanList.Zip(results, (plan, result) => new PlanResult(plan, result)))
                    {
                        BambooPlan bambooPlan = new BambooPlan();
                        if (planResult.Plan.IsBuilding)
                            bambooPlan.RemainingTime = planResult.Result.Progress.PrettyTimeRamaining;

                        if (oldResults.Values.Contains(planResult.Result))
                            continue;

                        oldResults[planResult.Plan.PlanKey.Key] = planResult.Result;
                        bambooPlan.BuildName = planResult.Plan.BuildName;
                        bambooPlan.PlanKey = planResult.Plan.PlanKey.Key;
                        bambooPlan.BuildState = planResult.Result.BuildState;
                        bambooPlan.ProjectKey = planResult.Plan.ProjectKey;
                        bambooPlan.ProjectName = planResult.Plan.ProjectName;
                        bambooPlan.IsBuilding = planResult.Plan.IsBuilding;
                        bambooPlan.IsEnabled = planResult.Plan.Enabled;
                        _bambooPlanPublisher.FirePlanChanged(bambooPlan);
                    }
                }

                Thread.Sleep(Timeout);
            }
        }

        private Task<Plans> GetFavouritePlans(Session session)
        {
            return GetResource<Plans>($"{_config.BambooHostname}/rest/api/latest/plan/?favourite&expand=plans.plan", session);
        }

        private async Task<Result> GetLatestBuild(Session session, string planKey)
        {
            Results latestResult = await GetResource<Results>($"{_config.BambooHostname}/rest/api/latest/result/{planKey}/?max-results=1&expand=results.result&includeAllStates=true", session).ConfigureAwait(false);
            int buildNumber = latestResult.ResultList.First().BuildNumber;
            //...{plankey}/latest?includeAllStates does not always return the latest result for some reason
            return await GetResource<Result>($"{_config.BambooHostname}/rest/api/latest/result/{planKey}/{buildNumber}", session).ConfigureAwait(false);
        }

        private async Task<T> GetResource<T>(string url, Session session) where T : class
        {
            IRestResponse<T> resultResponse = await _bambooClient.GetAsync<T>(url, session).ConfigureAwait(false);
            if (resultResponse.IsSuccess)
                return resultResponse.Data;

            if (resultResponse.StatusCode == HttpStatusCode.Unauthorized)
                _onSessionExpired.Invoke(this, EventArgs.Empty);

            return null;
        }

        private class PlanResult
        {
            public PlanResult(Plan plan, Result result)
            {
                Plan = plan;
                Result = result;
            }

            public Plan Plan { get; }
            public Result Result { get; }
        }
    }
}