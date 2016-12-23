using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Esynctraining.AdobeConnect
{
    [Serializable, XmlRoot(ElementName = "transcript")]
    [XmlType("transcript")]
    public class ChatTranscript
    {
        #region 

        public class Message
        {
            [XmlElement("text")]
            public string Text { get; set; }

            [XmlElement("timestamp")]
            public long TimeStamp { get; set; }

            [XmlElement("from")]
            public Recipient From { get; set; }

            [XmlElement("to")]
            public Recipient To { get; set; }
        }

        public class Recipient
        {
            [XmlElement("name")]
            public string Name { get; set; }

            [XmlElement("email")]
            public string Email { get; set; }

            [XmlElement("login")]
            public string Login { get; set; }

        }

        #endregion

        [XmlElement("meetingScoId")]
        public string MeetingScoId { get; set; }

        [XmlElement("meetingName")]
        public string MeetingName { get; set; }

        [XmlElement("meetingUrl")]
        public string MeetingUrl { get; set; }

        [XmlArray("messages")]
        [XmlArrayItem("message")]
        public Message[] Messages { get; set; }


        public ChatTranscript()
        {
            Messages = new Message[0];
        }

    }

}
