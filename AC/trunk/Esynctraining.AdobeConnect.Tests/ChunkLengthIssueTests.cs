using System;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class ChunkLengthIssueTests
    {
        [Test]
        public void RepeatChunkLengthIssue()
        {
            var sb = new StringBuilder();
            //sb.Append("text");
            //sb = null;
            //sb.Append("test");
            //Parallel.For(() =>
            //{
            //    sb.Append("test");
            //});
            Task.Run(() => sb.Append("test"));
            Task.Run(() => sb.Append("test"));
            Task.WaitAll();
            Console.WriteLine(sb);
        }
    }
}