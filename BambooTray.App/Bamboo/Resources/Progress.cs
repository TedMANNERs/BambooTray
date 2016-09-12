using System.Xml.Serialization;

namespace BambooTray.App.Bamboo.Resources
{
    [XmlRoot("progress")]
    public class Progress
    {
        [XmlElement("prettyTimeRemaining")]
        public string PrettyTimeRamaining { get; set; }

        protected bool Equals(Progress other)
        {
            return string.Equals(PrettyTimeRamaining, other.PrettyTimeRamaining);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Progress)obj);
        }

        public override int GetHashCode()
        {
            return (PrettyTimeRamaining != null ? PrettyTimeRamaining.GetHashCode() : 0);
        }
    }
}