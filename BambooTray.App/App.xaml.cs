using System;
using System.Windows;
using BambooTray.App.Bamboo.Resources;
using BambooTray.App.EventBroker;
using BambooTray.App.View;
using Hardcodet.Wpf.TaskbarNotification;
using Ninject;

namespace BambooTray.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppKernel.Instance.Kernel.Load<AppModule>();
            _trayIcon = (TaskbarIcon)FindResource("Popup");
            if (_trayIcon == null)
                return;

            ITrayIconViewModel trayIconViewModel = AppKernel.Get<ITrayIconViewModel>();
            _trayIcon.DataContext = trayIconViewModel;
            trayIconViewModel.PopupViewModel.BambooPlanChanged += BambooPlanChanged;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon.Dispose();
            base.OnExit(e);
        }

        public void BambooPlanChanged(object sender, PlanEventArgs e)
        {
            if (e.Plan.IsBuilding)
                _trayIcon.ShowBalloonTip("Building", $"{e.Plan.ProjectName} {e.Plan.BuildName} is building", BambooTray.App.Properties.Resources.icon_building_06);
            else
                switch (e.Plan.BuildState)
                {
                    case BuildState.Failed:
                        _trayIcon.ShowBalloonTip("Build Failed", $"{e.Plan.ProjectName} {e.Plan.BuildName} failed", BambooTray.App.Properties.Resources.icon_build_failed);
                        break;
                    case BuildState.Successful:
                        _trayIcon.ShowBalloonTip("Build succeeded", $"{e.Plan.ProjectName} {e.Plan.BuildName} was successful", BambooTray.App.Properties.Resources.icon_build_successful);
                        break;
                    case BuildState.Unknown:
                        _trayIcon.ShowBalloonTip("Build stopped", $"{e.Plan.ProjectName} {e.Plan.BuildName} was stopped", BambooTray.App.Properties.Resources.icon_build_unknown);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
    }
}