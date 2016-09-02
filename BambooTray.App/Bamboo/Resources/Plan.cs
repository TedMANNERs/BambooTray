using System.Xml.Serialization;

namespace BambooTray.App.Bamboo.Resources
{
    [XmlRoot("plan")]
    public class Plan
    {
        [XmlElement("projectKey")]
        public string ProjectKey { get; set; }
        [XmlElement("projectName")]
        public string ProjectName { get; set; }
        [XmlElement("buildName")]
        public string BuildName { get; set; }
        [XmlElement("isActive")]
        public bool IsActive { get; set; }
        [XmlElement("isBuilding")]
        public bool IsBuilding { get; set; }
        [XmlElement("averageBuildTimeInSeconds")]
        public float AverageBuildTimeInSeconds { get; set; }

        protected bool Equals(Plan other)
        {
            return string.Equals(ProjectKey, other.ProjectKey) &&
                   string.Equals(ProjectName, other.ProjectName) &&
                   string.Equals(BuildName, other.BuildName) &&
                   IsActive == other.IsActive &&
                   IsBuilding == other.IsBuilding &&
                   AverageBuildTimeInSeconds.Equals(other.AverageBuildTimeInSeconds);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Plan)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (ProjectKey != null ? ProjectKey.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ProjectName != null ? ProjectName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BuildName != null ? BuildName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsActive.GetHashCode();
                hashCode = (hashCode * 397) ^ IsBuilding.GetHashCode();
                hashCode = (hashCode * 397) ^ AverageBuildTimeInSeconds.GetHashCode();
                return hashCode;
            }
        }
    }
}