using System;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;
using System.ComponentModel.DataAnnotations;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class MeetingItemDto
    {
        [Required]
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Meeting's url-path.
        /// </summary>
        [Required]
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// Meeting's sco-id
        /// </summary>
        [Required]
        [DataMember]
        public string ScoId { get; set; }


        public static MeetingItemDto Build(MeetingItem arg)
        {
            if (arg == null)
                throw new ArgumentNullException(nameof(arg));

            return new MeetingItemDto
            {
                Name = arg.Name,
                Url = arg.UrlPath,
                ScoId = arg.ScoId,
            };
        }


        public static MeetingItemDto Build(TrainingItem arg)
        {
            if (arg == null)
                throw new ArgumentNullException(nameof(arg));

            return new MeetingItemDto
            {
                Name = arg.Name,
                Url = arg.UrlPath,
                ScoId = arg.ScoId,
            };
        }

    }

}
