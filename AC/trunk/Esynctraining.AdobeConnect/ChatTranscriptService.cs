using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;
using System.Text;

namespace Esynctraining.AdobeConnect
{
    public class ChatTranscriptService : IChatTranscriptService
    {
        private static readonly IEnumerable<ScoShortcutType> TranscriptShortcut = new ScoShortcutType[] { ScoShortcutType.chat_transcripts };
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ChatTranscript));


        private readonly IContentService _contentService;
        private readonly IAdobeConnectProxy _proxy;
        private readonly ILogger _logger;


        public ChatTranscriptService(IAdobeConnectProxy proxy, IContentService contentService, ILogger logger)
        {
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public ScoShortcut GetChatTranscriptsFolder(string accountId)
        {
            IEnumerable<ScoShortcut> shortcuts = _contentService.GetShortcuts(TranscriptShortcut);

            if (shortcuts.Count() != 1)
                throw new InvalidOperationException("Chat shortcuts are not enabled for Adobe Connect.");

            var chatsFolder = shortcuts.Single(x => x.Type == ScoShortcutType.chat_transcripts.ToString().Replace('_', '-'));
            return chatsFolder;
        }


        public ChatTranscript GetMeetingChatTranscript(string accountId, string meetingScoId, DateTime sessionDateCreated, DateTime sessionDateEnd)
        {
            ScoShortcut chatsFolder = GetChatTranscriptsFolder(accountId);

            IEnumerable<ScoContent> chatScoList = _contentService.GetFolderContent(chatsFolder.ScoId, meetingScoId);

            // HACK: 
            //if (chatScoList.Count() != 1)
            //    throw new NotImplementedException();

            //(StartA <= EndB) and (EndA >= StartB)

            var chatFile = chatScoList.Where(x => x.BeginDate <= sessionDateEnd && x.EndDate >= sessionDateCreated).SingleOrDefault();
            if (chatFile == null)
                //TODO: what to do
                return null;

            var chatFileScoId = chatFile.ScoId;

            // HACK: 
            //var chatFileScoId = chatScoList.Single().ScoId;
            //var chatFileScoId = chatScoList.Last().ScoId;

            string err;
            byte[] zipContent = _proxy.GetContent(chatFileScoId, out err);

            using (var chatXmlFileStream = new MemoryStream())
            {
                using (var compressedFileStream = new MemoryStream(zipContent))
                {
                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Read, true))
                    {
                        ZipArchiveEntry zipEntry;
                        try
                        {
                            zipEntry = zipArchive.Entries.Single();
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Error parsing chat transcription. No transcript file found. ChatFile sco-id:{chatFileScoId}.", ex);
                        }

                        using (var zipEntryStream = zipEntry.Open())
                        {
                            zipEntryStream.CopyTo(chatXmlFileStream);
                        }
                    }
                }

                chatXmlFileStream.Position = 0;

                try
                {
                    using (var sr = new StreamReader(chatXmlFileStream, Encoding.UTF8))
                    {
                        var chat = (ChatTranscript)Serializer.Deserialize(sr);

                        if (chat.Messages.Any(x => x.To == null))
                            _logger.Error($"Message with To==null. MeetingScoId:'{meetingScoId}'. ChatFile sco-id:{chatFileScoId}.");

                        return chat;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error parsing chat transcription. MeetingScoId:'{meetingScoId}'. ChatFile sco-id:{chatFileScoId}. {_proxy.AdobeConnectRoot}.", ex);
                    throw new InvalidOperationException("Error parsing chat transcription.", ex);
                }
            }
        }

    }

}
