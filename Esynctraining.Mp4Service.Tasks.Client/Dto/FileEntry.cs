namespace Esynctraining.Mp4Service.Tasks.Client.Dto
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

}
