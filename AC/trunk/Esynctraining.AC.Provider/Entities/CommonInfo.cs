using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    //todo:should implement all common info fields
    [Serializable]
    [XmlRoot("common")]
    public class CommonInfo
    {
        [XmlElement("host")]
        public string AccountUrl { get; set; }
    }
}
