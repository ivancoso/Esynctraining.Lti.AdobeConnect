using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;

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
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));
            if (contentService == null)
                throw new ArgumentNullException(nameof(contentService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _contentService = contentService;
            _proxy = proxy;
            _logger = logger;
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
                            throw new InvalidOperationException("Error parsing chat transcription. No transcript file found.", ex);
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
                    var chat = (ChatTranscript)Serializer.Deserialize(chatXmlFileStream);
                    return chat;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error parsing chat transcription. MeetingScoId:'{meetingScoId}'. {_proxy.AdobeConnectRoot}.", ex);
                    throw new InvalidOperationException("Error parsing chat transcription.", ex);
                }
            }
        }

    }

}
