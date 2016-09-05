using System.Collections.Generic;
using System.Xml.Serialization;

namespace BambooTray.App.Bamboo.Resources
{
    [XmlRoot("plans")]
    public class Plans
    {
        [XmlElement("link")]
        public Link Link { get; set; }

        [XmlArray("plans")]
        [XmlArrayItem("plan")]
        public List<Plan> PlanList { get; set; }
    }
}