using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class ScoContentDto
    {
        [Required]
        [DataMember]
        public string ScoId { get; set; }

        //[DataMember]
        //public int FolderId { get; set; }

        //[DataMember]
        //public string Type { get; set; }

        [Required]
        [DataMember]
        public string Icon { get; set; }

        [IgnoreDataMember]
        public bool IsFolder { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        [IgnoreDataMember]
        public DateTime BeginDate { get; set; }

        [IgnoreDataMember]
        public DateTime DateModified { get; set; }

        [IgnoreDataMember]
        public DateTime EndDate { get; set; }
        
        [DataMember]
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

        [DataMember]
        public int ByteCount { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
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
                Description = sco.Description,
            };
        }

    }

    public class ScoContentDtoProcessor : IDtoProcessor<ScoContentDto>
    {
        private readonly IContentService _contentService;
        private readonly string _breezeToken;


        public ScoContentDtoProcessor(IContentService contentService, string breezeToken)
        {
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
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