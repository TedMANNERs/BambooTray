using System.Collections.Generic;
using System.Xml.Serialization;

namespace BambooTray.App.Configuration
{
    public class Configuration
    {
        public Configuration()
        {
            Plans = new List<string>();
        }

        [XmlArray]
        [XmlArrayItem(ElementName = "Plan")]
        public List<string> Plans { get; set; }
        public string BambooHostname { get; set; }
    }
}