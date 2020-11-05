using System;
using System.Collections.Generic;
using System.Text;

namespace Esynctraining.Mail
{
    public class ContentType
    {
        public string MediaSubtype { get; set; }
        public string MediaType { get; set; }
        public Dictionary<string, string> Parameters { get; }

        public ContentType(string mediaType, string mediaSubtype)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
            {
                throw new ArgumentException("message", nameof(mediaType));
            }

            if (string.IsNullOrWhiteSpace(mediaSubtype))
            {
                throw new ArgumentException("message", nameof(mediaSubtype));
            }

            MediaType = mediaType;
            MediaSubtype = mediaSubtype;
            Parameters = new Dictionary<string, string>();
        }
    }
}
