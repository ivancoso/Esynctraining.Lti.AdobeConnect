using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class RecordingJobParser
    {
        public static RecordingJob Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new RecordingJob()
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    SourceScoId = xml.SelectAttributeValue("source-sco-id"),
                    FolderId = xml.SelectAttributeValue("folder-id"),
                    Duration = xml.ParseAttributeInt("duration"),
                    AccountId = xml.SelectAttributeValue("account-id"),
                    CreditMinute = xml.ParseAttributeInt("credit-minute"),
                    DebitMinute = xml.ParseAttributeInt("debit-minute"),
                    EncoderServiceJobProgress = xml.ParseAttributeInt("encoder-service-job-progress"),
                    JobId = xml.SelectAttributeValue("job-id"),
                    JobStatus = xml.SelectAttributeValue("job-status"),
                    PrincipalId = xml.SelectAttributeValue("principal-id"),
                    RetryNumber = xml.ParseAttributeInt("retry-number"),
                    DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now),
                    DateModified = xml.ParseNodeDateTime("date-modified/text()", DateTime.Now),
                    EncoderServiceJobId = xml.SelectSingleNodeValue("encoder-service-job-id/text()"),
                    EncoderServiceJobParams = xml.SelectSingleNodeValue("encoder-service-job-params/text()"),
                    EncoderServiceJobStatus = xml.SelectSingleNodeValue("encoder-service-job-status/text()"),
                    RecServiceHostName = xml.SelectSingleNodeValue("rec-service-host-name/text()"),
                    RecServiceIpAddress = xml.SelectSingleNodeValue("rec-service-ip-address/text()")
                };
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
