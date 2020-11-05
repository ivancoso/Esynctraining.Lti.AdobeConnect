using System.IO;

namespace Esynctraining.Mail
{
    public class Attachment
    {
        public string FileName { get; set; }

        public Stream Stream { get; set; }

        public ContentType ContentType { get; set; }

    }

}
