namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    internal static class ScoInfoByUrlParser
    {
        public static ScoInfoByUrl Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            var item = new ScoInfoByUrl();
            ScoInfoParser.Parse(xml.SelectSingleNode("//sco"), item);
            ParseOwner(xml.SelectSingleNode("//owner-principal"), item);
            return item;
        }


        private static void ParseOwner(XmlNode xml, ScoInfoByUrl info)
        {
            if (xml == null || xml.Attributes == null)
            {
                return;
            }

            info.Owner = new ScoInfoByUrl.OwnerPrincipal
            {
                PrincipalId = xml.SelectAttributeValue("principal-id"),
                AccountId = xml.SelectAttributeValue("account-id"),
                IsHidden = xml.ParseAttributeBool("is-hidden"),
                IsPrimary = xml.ParseAttributeBool("is-primary"),
                HasChildren = xml.ParseAttributeBool("has-children"),
                //Type = xml.SelectAttributeValue("type"),
                Login = xml.SelectSingleNodeValue("login/text()"),
                Name = xml.SelectSingleNodeValue("name/text()"),
                Email = xml.SelectSingleNodeValue("email/text()"),
                //DisplayId = xml.SelectSingleNodeValue("display-uid/text()"),
                //Description = xml.SelectSingleNodeValue("description/text()"),
            };
        }

    }

}
