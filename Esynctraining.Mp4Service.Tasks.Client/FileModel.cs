using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Mp4Service.Tasks.Client.Dto;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public sealed class FileModel : ModelBase
    {
        public class FileEntry
        {
            public string FileName { get; set; }

            public byte[] Content { get; set; }
            

            public FileEntry(byte[] content, string fileName)
            {
                Content = content;
                FileName = fileName;
            }

        }


        private readonly IAdobeConnectAccess _access;

        public FileModel(ILogger logger, IAdobeConnectAccountService acAccountService, IAdobeConnectAccess access) 
            : base(logger, acAccountService, access)
        {
        }
        

        public IEnumerable<FileEntry> GetFiles(IEnumerable<string> fileScoIdList)
        {
            if (fileScoIdList == null)
                throw new ArgumentNullException("fileScoIdList");
            
            var result = new List<FileEntry>();
            foreach (string fileScoId in fileScoIdList)
            {
                ScoInfoResult fileSco = _ac.GetScoInfo(fileScoId);
                if (!fileSco.Success)
                {
                    throw new Exception("File not found.");
                }
                if (fileSco.ScoInfo.Type != ScoType.content)
                {
                    throw new Exception("File not found.");
                }

                result.Add(GetOriginalFileContent(fileSco.ScoInfo));
            }
            
            return result;
        }

        public ScoInfoResult GetFileSco(string fileScoId)
        {
            ScoInfoResult fileSco = _ac.GetScoInfo(fileScoId);
            if (!fileSco.Success)
            {
                throw new Exception("File not found.");
            }
            if (fileSco.ScoInfo.Type != ScoType.content)
            {
                throw new Exception("File not found.");
            }

            return fileSco;
        }

        public FileDto Create(string fileScoId, string fileName, string fileContentType, byte[] content)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName can't be empty", "fileName");
            if (string.IsNullOrWhiteSpace(fileContentType))
                throw new ArgumentException("fileContentType can't be empty", "fileContentType");
            if (content == null)
                throw new ArgumentNullException("content");
            
            var uploadScoInfo = new UploadScoInfo
            {
                scoId = fileScoId,
                fileContentType = fileContentType,
                fileName = fileName,
                fileBytes = content,
                title = fileName,
            };

            try
            {
                string originalFileName = fileName;
                StatusInfo uploadResult = _ac.UploadContent(uploadScoInfo);
            }
            catch (AdobeConnectException ex)
            {
                // Status.Code: invalid. Status.SubCode: format. Invalid Field: file
                if (ex.Status.Code == StatusCodes.invalid && ex.Status.SubCode == StatusSubCodes.format && ex.Status.InvalidField == "file")
                    throw new Exception("Invalid file format selected.");

                throw new Exception("Error occured during file uploading.", ex);
            }

            return new FileDto
            {
                Id = fileScoId,
                Name = fileName,
                Size = content.Length,
            };
        }


        private FileEntry GetOriginalFileContent(ScoInfo file)
        {
            string error;
            byte[] content = _ac.GetContentByUrlPath(file.UrlPath, "zip", out error);
            
            var archive = new ZipArchive(new MemoryStream(content));
            ZipArchiveEntry fileEntry = archive.Entries[0];

            byte[] fileContent;
            using (var memoryStream = new MemoryStream())
            {
                fileEntry.Open().CopyTo(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            return new FileEntry(fileContent, fileEntry.Name);
        }

    }

}
