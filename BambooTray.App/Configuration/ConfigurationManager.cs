using System.IO;
using System.Xml.Serialization;

namespace BambooTray.App.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private const string FileName = "AppConfiguration.xml";
        private const string Dir = "Configuration/";
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(Configuration));
        public Configuration Config { get; set; }

        public void Load()
        {
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);

            if (File.Exists(Path.Combine(Dir, FileName)))
            {
                using (FileStream stream = new FileStream(Path.Combine(Dir, FileName), FileMode.Open))
                    Config = (Configuration)_serializer.Deserialize(stream);
            }
            else
            {
                Config = new Configuration();
                Save();
            }
        }

        public void Save()
        {
            using (FileStream stream = new FileStream(Path.Combine(Dir, FileName), FileMode.OpenOrCreate))
                _serializer.Serialize(stream, Config);
        }
    }
}