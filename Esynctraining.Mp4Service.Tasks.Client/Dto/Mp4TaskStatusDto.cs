using System;
using System.Runtime.Serialization;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace Esynctraining.Mp4Service.Tasks.Client.Dto
{
    [DataContract]
    public sealed class Mp4TaskStatusDto
    {
        private static readonly string _baseMp4AccessUrl;
        private static readonly string _baseVttAccessUrl;


        [DataMember(Name = "mp4_sco_id")]
        public string mp4_sco_id { get; set; }

        [DataMember(Name = "cc_sco_id")]
        public string cc_sco_id { get; set; }

        [DataMember(Name = "status")]
        public string status { get; set; }


        [DataMember(Name = "mp4_url")]
        public string mp4_url
        {
            get
            {
                if (string.IsNullOrWhiteSpace(mp4_sco_id))
                    return null;
                return string.Format(_baseMp4AccessUrl, mp4_sco_id);
            }
            set
            {
            }
        }

        [DataMember(Name = "cc_ulr")]
        public string cc_ulr
        {
            get
            {
                if (string.IsNullOrWhiteSpace(cc_sco_id))
                    return null;
                return string.Format(_baseVttAccessUrl, cc_sco_id);
            }
            set
            {
            }
        }


        static Mp4TaskStatusDto()
        {
            var settings = IoC.Resolve<ApplicationSettingsProvider>() as dynamic;
            _baseMp4AccessUrl = settings.Mp4FileAccess_Mp4 as string;
            _baseVttAccessUrl = settings.Mp4FileAccess_Vtt as string;

            if (string.IsNullOrWhiteSpace(_baseMp4AccessUrl))
                throw new InvalidOperationException("Mp4FileAccess_Mp4 should have have");
            Uri result;
            if (!Uri.TryCreate(_baseMp4AccessUrl, UriKind.Absolute, out result))
                throw new ArgumentException("Mp4FileAccess_Mp4 should have Absolute Url value");

            if (string.IsNullOrWhiteSpace(_baseVttAccessUrl))
                throw new InvalidOperationException("Mp4FileAccess_Vtt should have have");
            if (!Uri.TryCreate(_baseVttAccessUrl, UriKind.Absolute, out result))
                throw new ArgumentException("Mp4FileAccess_Vtt should have Absolute Url value");
        }

    }
}
