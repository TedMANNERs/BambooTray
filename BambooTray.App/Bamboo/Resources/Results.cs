using System.Collections.Generic;
using System.Xml.Serialization;

namespace BambooTray.App.Bamboo.Resources
{
    [XmlRoot("results")]
    public class Results
    {
        [XmlElement("link")]
        public Link Link { get; set; }

        [XmlArray("results")]
        [XmlArrayItem("result")]
        public List<Result> ResultList { get; set; }
    }
}