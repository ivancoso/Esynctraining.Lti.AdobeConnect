using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class RecordingParser
    {
        public static Recording Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new Recording
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    SourceScoId = xml.SelectAttributeValue("source-sco-id"),
                    FolderId = xml.ParseAttributeInt("folder-id"),
                    Type = xml.SelectAttributeValue("type"),
                    Icon = xml.SelectAttributeValue("icon"),
                    DisplaySequence = xml.ParseAttributeInt("display-seq"),
                    Duration = xml.SelectSingleNodeValue("duration/text()"),
                    IsFolder = xml.ParseAttributeInt("is-folder") != 0,
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    Description = xml.SelectSingleNodeValue("description/text()"),
                    UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                    BeginDate = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                    EndDate = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                    DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now),
                    DateModified = xml.ParseNodeDateTime("date-modified/text()", DateTime.Now),
                    EncoderServiceJobParams = xml.SelectSingleNodeValue("encoder-service-job-params/text()"),
                    EncoderServiceJobStatus = xml.SelectSingleNodeValue("encoder-service-job-status/text()"),
                    Filename = xml.SelectSingleNodeValue("filename/text()"),
                    NoOfDownloads = xml.ParseAttributeInt("no-of-downloads"),
                    EncoderServiceJobId = xml.SelectSingleNodeValue("encoder-service-job-id"),
                    EncoderServiceJobProgress = xml.ParseAttributeInt("encoder-service-job-progress"),
                    JobStatus = xml.SelectAttributeValue("job-status"),
                    AccountId = xml.SelectAttributeValue("account-id"),
                    JobId = xml.SelectAttributeValue("job-id"),
                    RecordingEdited = xml.ParseNodeBool("recording-edited/text()"),
                    RecordingEditedDuration = xml.SelectSingleNodeValue("recording-edited-duration/text()"),
                    AfRecordingDuration = xml.SelectSingleNodeValue("af-recording-duration/text()"),

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
