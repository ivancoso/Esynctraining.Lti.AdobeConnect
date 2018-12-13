namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;

    /// <summary>
    /// The Event Info parser.
    /// </summary>
    internal static class EventInfoParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>EventInfo object.</returns>
        public static EventInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var ei = new EventInfo
                             {
                                 ScoId = xml.SelectAttributeValue("sco-id"),
                                 Name = xml.SelectSingleNodeValue("name/text()"),
                                 DomainName = xml.SelectSingleNodeValue("domain-name/text()"),
                                 UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                                 DateBegin = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                                 DateEnd = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                                 Expired = xml.ParseNodeBool("expired/text()"),
                                 SpeakerName = xml.SelectSingleNodeValue("speaker-name/text()"),
                                 Info = xml.SelectSingleNodeValue("event-info/text()"),
                                 SpeakerImage = xml.SelectSingleNodeValue("speaker-image/text()"),
                                 SpeakerBriefOverview = xml.SelectSingleNodeValue("speaker-brief-overview/text()"),
                                 SpeakerDetailedOverview = xml.SelectSingleNodeValue("speaker-detailed-overview/text()"),
                                 PasswordBypass = xml.ParseNodeBool("password-bypass/text()"),
                };

                ei.Duration = ei.DateEnd.Subtract(ei.DateBegin);

                // if mDetail.DateBegin is not defined and duration is 0 => then this is the folder which should be ignored
                if (!ei.DateBegin.Equals(default(DateTime)) || ei.Duration.TotalMinutes != 0)
                {
                    return ei;
                }
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
/*
 <results>
<status code="ok"/>
<event-info>
<account-id>7</account-id>
<account-name>Enterprise Account</account-name>
<date-begin>2018-12-20T02:30:00.000-08:00</date-begin>
<date-end>2018-12-20T03:30:00.000-08:00</date-end>
<date-modified>2018-12-13T02:45:09.447-08:00</date-modified>
<domain-name>https://connectstage.esynctraining.com</domain-name>
<event-category>live</event-category>
<event-guest-policy>guest</event-guest-policy>
<event-status>not-cancelled</event-status>
<group-id>1890593</group-id>
<is-registration-limit-enabled>false</is-registration-limit-enabled>
<is-social-enabled>false</is-social-enabled>
<login-is-email>false</login-is-email>
<login-url>/_a7/e10idmt6gyw/event/login.html</login-url>
<name>BC-464 Without password</name>
<number-registered-users>1</number-registered-users>
<password-bypass>true</password-bypass>
<registration-login-url>/_a7/e10idmt6gyw/event/registration_login.html</registration-login-url>
<registration-type>advance</registration-type>
<require-approval>false</require-approval>
<sc-tracking-sco-name-signature>BC-464 Without password(1890591)</sc-tracking-sco-name-signature>
<type>meeting</type>
<url-path>/e10idmt6gyw/</url-path>
</event-info>
<preferences acl-id="1890591" lang="en" time-zone-id="4"/>
</results>
 */
