using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Esynctraining.Core.Extensions;

namespace Esynctraining.AdobeConnect
{
    [Serializable, XmlRoot(ElementName = "transcript")]
    [XmlType("transcript")]
    public class ChatTranscript
    {
        #region Inner Class: Message

        public class Message
        {
            [XmlElement("text")]
            public string Text { get; set; }

            [XmlElement("timestamp")]
            public long TimeStamp { get; set; }

            [XmlElement("from")]
            public Recipient From { get; set; }

            [XmlElement("to")]
            public Recipient[] To { get; set; }

            public DateTime Date
            {
                get { return ((double)TimeStamp).ConvertFromUnixTimeStamp();  }
            }


            public Message()
            {
                To = new Recipient[0];
            }

        }

        #endregion

        #region Inner Class: Recipient

        public class Recipient : IEquatable<Recipient>
        {
            [XmlElement("name")]
            public string Name { get; set; }

            [XmlElement("email")]
            public string Email { get; set; }

            [XmlElement("login")]
            public string Login { get; set; }


            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                Recipient recipientObj = obj as Recipient;
                if (recipientObj == null)
                    return false;
                return Equals(recipientObj);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            public override string ToString()
            {
                return $"{Name}.{Login}.{Email}";
            }

            public bool Equals(Recipient other)
            {
                if (other == null)
                    return false;

                if (Name != other.Name)
                    return false;
                // TRICK: crazy AC issue: https://monosnap.com/file/vVGC0M5LVEFiZPF5sm57KLlqfcvJxJ
                //if (Login != other.Login)
                //    return false;
                //if (Email != other.Email)
                //    return false;

                return true;
            }

        }

        #endregion

        #region Inner Class: PrivateMessagePair

        public class PrivateMessagePair
        {
            public Recipient From { get; }

            public Recipient To { get; }


            internal PrivateMessagePair(Recipient from, Recipient to)
            {
                From = from;
                To = to;
            }


            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                PrivateMessagePair recipientObj = obj as PrivateMessagePair;
                if (recipientObj == null)
                    return false;
                return Equals(recipientObj);
            }

            public override int GetHashCode()
            {
                string[] names = new string[2] { From.Name, To.Name };
                Array.Sort(names);
                return string.Join(",", names).GetHashCode();
            }

            public override string ToString()
            {
                //return $"{To.Name}-{From.Name}";
                string[] names = new string[2] { From.Name, To.Name };
                Array.Sort(names);
                return string.Join(",", names);
            }

            public bool Equals(PrivateMessagePair other)
            {
                if (other == null)
                    return false;

                if (!From.Equals(other.From) && !From.Equals(other.To))
                    return false;
                if (!To.Equals(other.To) && !To.Equals(other.From))
                    return false;

                return true;
            }
        }

        #endregion

        #region Inner Class: PrivateMessage

        private sealed class PrivateMessage : Message
        {
            public PrivateMessagePair RecipientPair { get; }


            public PrivateMessage(Message source)
            {
                From = source.From;
                To = source.To;
                Text = source.Text;
                TimeStamp = source.TimeStamp;
                RecipientPair = new PrivateMessagePair(source.From, source.To.Single());
            }

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


        public IEnumerable<Message> GetPublicChat()
        {
            return Messages.Where(x => !(x.To.Count() == 1 && x.To.Single().Name != x.From.Name)).OrderBy(x => x.TimeStamp);
        }
        
        public IDictionary<PrivateMessagePair, IEnumerable<Message>> GetPrivateChatGroups()
        {
            var allPrivate = GetPrivateChats().Select(key => new PrivateMessage(key));

            var grouped = allPrivate
                .GroupBy(item => item.RecipientPair)
                .ToDictionary(g => g.Key, g => g.Cast<Message>());

            return grouped;
        }


        private IEnumerable<Message> GetPrivateChats()
        {
            return Messages.Where(x => (x.To.Count() == 1 && x.To.Single().Name != x.From.Name)).OrderBy(x => x.TimeStamp);
        }

    }

}
