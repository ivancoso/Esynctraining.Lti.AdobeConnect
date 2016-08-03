using System;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    public class GeneratedRecordingJob : Recording
    {
        [XmlElement("job-date-created")]
        public DateTime JobDateCreated { get; set; }

        [XmlElement("job-date-modified")]
        public DateTime JobDateModified { get; set; }

    }

}
