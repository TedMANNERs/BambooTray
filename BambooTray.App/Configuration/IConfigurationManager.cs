namespace BambooTray.App.Configuration
{
    public interface IConfigurationManager
    {
        Configuration Config { get; set; }

        void Load();

        void Save();
    }
}