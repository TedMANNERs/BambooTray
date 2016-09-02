using System.Xml.Serialization;

namespace BambooTray.App.Bamboo.Resources
{
    [XmlRoot("result")]
    public class Result
    {
        [XmlElement("buildResultKey")]
        public string BuildResultKey { get; set; }
        [XmlElement("buildState")]
        public BuildState BuildState { get; set; }
        [XmlElement("buildNumber")]
        public int BuildNumber { get; set; }
        [XmlElement("buildDurationInSeconds")]
        public int BuildDurationInSeconds { get; set; }

        protected bool Equals(Result other)
        {
            return string.Equals(BuildResultKey, other.BuildResultKey) &&
                   BuildState == other.BuildState &&
                   BuildNumber == other.BuildNumber &&
                   BuildDurationInSeconds == other.BuildDurationInSeconds;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Result)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (BuildResultKey != null ? BuildResultKey.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)BuildState;
                hashCode = (hashCode * 397) ^ BuildNumber;
                hashCode = (hashCode * 397) ^ BuildDurationInSeconds;
                return hashCode;
            }
        }
    }
}