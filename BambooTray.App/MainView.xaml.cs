using System;
using System.Windows;
using BambooTray.App.Bamboo.Resources;
using BambooTray.App.EventBroker;

namespace BambooTray.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IMainViewModel mainViewModel = (MainViewModel)DataContext;
            mainViewModel.PopupViewModel.BambooPlanChanged += BambooPlanChanged;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            IMainViewModel mainViewModel = (MainViewModel)DataContext;
            mainViewModel.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void BambooPlanChanged(object sender, PlanEventArgs e)
        {
            if (e.Plan.IsBuilding)
                TaskbarIcon.ShowBalloonTip("Building", $"{e.Plan.ProjectName} {e.Plan.BuildName} is building", Properties.Resources.icon_building_06);
            else
                switch (e.Plan.BuildState)
                {
                    case BuildState.Failed:
                        TaskbarIcon.ShowBalloonTip("Build Failed", $"{e.Plan.ProjectName} {e.Plan.BuildName} failed", Properties.Resources.icon_build_failed);
                        break;
                    case BuildState.Successful:
                        TaskbarIcon.ShowBalloonTip("Build succeded", $"{e.Plan.ProjectName} {e.Plan.BuildName} was successful", Properties.Resources.icon_build_successful);
                        break;
                    case BuildState.Unknown:
                        TaskbarIcon.ShowBalloonTip("Build stopped", $"{e.Plan.ProjectName} {e.Plan.BuildName} was stopped", Properties.Resources.icon_build_unknown);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
    }
}