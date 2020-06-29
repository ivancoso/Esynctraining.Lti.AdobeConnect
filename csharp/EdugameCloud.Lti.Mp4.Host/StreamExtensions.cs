using System.IO;

namespace EdugameCloud.Lti.Mp4.Host
{
    // TODO: move to reusable place. core.only?
    internal static class StreamExtensions
    {
        public static byte[] ReadFully(this Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

    }

}
