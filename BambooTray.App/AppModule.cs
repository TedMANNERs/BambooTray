using BambooTray.App.Configuration;
using Ninject.Modules;

namespace BambooTray.App
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConfigurationManager>().To<ConfigurationManager>().InSingletonScope().OnActivation(x => x.Load());
        }
    }
}