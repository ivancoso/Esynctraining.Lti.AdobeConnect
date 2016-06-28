using System;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.WebApi;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.Content.Host.Dto
{
    [DataContract]
    public class ScoContentDto
    {
        [DataMember(Name = "sco_id")]
        public string ScoId { get; set; }

        //[DataMember]
        //public int FolderId { get; set; }

        //[DataMember]
        //public string Type { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "isFolder")]
        public bool IsFolder { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime DateModified { get; set; }

        public DateTime EndDate { get; set; }

        //[DataMember(Name = "start_timestamp")]
        //public long? StartTimestamp
        //{
        //    get
        //    {
        //        if (IsFolder)
        //            return null;
        //        if (BeginDate == DateTime.MinValue)
        //            return null;

        //        return (long)BeginDate.ConvertToUnixTimestamp();
        //    }
        //    set
        //    {
        //    }
        //}

        [DataMember(Name = "duration")]
        public string Duration
        {
            get
            {
                if (IsFolder)
                    return null;
                if (EndDate == DateTime.MinValue && EndDate == DateTime.MinValue)
                    return null;

                return (EndDate - BeginDate).ToString(@"h\:mm");
            }
            set
            {
            }
        }

        [DataMember(Name = "byteCount")]
        public int ByteCount { get; set; }

        [DataMember(Name = "modifiedAt")]
        public long? ModifiedAt
        {
            get
            {
                if (DateModified == DateTime.MinValue)
                    return null;

                return (long)DateModified.ConvertToUnixTimestamp();
            }
            set
            {
            }
        }

    }

    public class ScoContentDtoMapper : IScoContentDtoMapper<ScoContentDto>
    {
        public ScoContentDto Map(ScoContent sco)
        {
            if (sco == null)
                throw new ArgumentNullException(nameof(sco));

            return new ScoContentDto
            {
                ScoId = sco.ScoId,
                Name = sco.Name,
                IsFolder = sco.IsFolder,
                Icon = sco.Icon,
                BeginDate = sco.BeginDate,
                EndDate = sco.EndDate,
                DateModified = sco.DateModified,
                ByteCount = sco.ByteCount,
            };
        }

    }

    public class ScoContentDtoProcessor : IDtoProcessor<ScoContentDto>
    {
        private readonly IContentService _contentService;
        private readonly string _breezeToken;


        public ScoContentDtoProcessor(IContentService contentService, string breezeToken)
        {
            if (contentService == null)
                throw new ArgumentNullException(nameof(contentService));

            _contentService = contentService;
            _breezeToken = breezeToken;
        }


        public ScoContentDto Process(ScoContentDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            //if (dto.IsFolder)
            //    dto.DownloadLink = _contentService.GetDownloadAsZipLink(dto.ScoId, _breezeToken);

            return dto;
        }

    }

}