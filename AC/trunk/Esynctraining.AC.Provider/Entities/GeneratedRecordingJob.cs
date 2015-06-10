using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    [Serializable]
    public class GeneratedRecordingJob : Recording
    {
        [XmlElement("duration")]
        public string ExactRecordingDuration { get; set; }

        [XmlElement("job-date-created")]
        public DateTime JobDateCreated { get; set; }

        [XmlElement("job-date-modified")]
        public DateTime JobDateModified { get; set; }
    }
}
