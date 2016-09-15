using System.Xml.Serialization;

namespace BambooTray.App.Bamboo.Resources
{
    [XmlRoot("result")]
    public class Result
    {
        [XmlAttribute("number")]
        public int Number { get; set; }

        [XmlElement("buildResultKey")]
        public string BuildResultKey { get; set; }

        [XmlElement("buildState")]
        public BuildState BuildState { get; set; }

        [XmlElement("buildNumber")]
        public int BuildNumber { get; set; }

        [XmlElement("buildDurationInSeconds")]
        public int BuildDurationInSeconds { get; set; }

        [XmlElement("progress")]
        public Progress Progress { get; set; }

        protected bool Equals(Result other)
        {
            return Number == other.Number
                   && string.Equals(BuildResultKey, other.BuildResultKey)
                   && BuildState == other.BuildState
                   && BuildNumber == other.BuildNumber
                   && BuildDurationInSeconds == other.BuildDurationInSeconds
                   && Equals(Progress, other.Progress);
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
                int hashCode = Number;
                hashCode = (hashCode * 397) ^ (BuildResultKey != null ? BuildResultKey.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)BuildState;
                hashCode = (hashCode * 397) ^ BuildNumber;
                hashCode = (hashCode * 397) ^ BuildDurationInSeconds;
                hashCode = (hashCode * 397) ^ (Progress != null ? Progress.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}