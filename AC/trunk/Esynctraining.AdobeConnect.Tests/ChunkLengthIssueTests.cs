using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class ChunkLengthIssueTests
    {
        private static readonly StringBuilder StackTrace = new StringBuilder();


        private static readonly object Locker = new object();

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
            //var a = Task.Run(() => sb.Append("test"));
            //var b = Task.Run(() => sb.Append("test"));
            //Task.WaitAll(a, b);
            var tasks = new List<Task>();
            for (var i = 0; i < 4000; i++)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    TraceTest(new MyException()
                    {
                        InnerException = 
                        {
                            InnerException = { Data = { } }
                        }
                    });
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
            //Console.WriteLine(sb);
        }

        public static string TraceTest(Exception ex)
        {
            var exceptionStack = new StringBuilder();

            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                exceptionStack.AppendFormat("{0}\n{1}\n", ex.Message, ex.StackTrace);
            }
            else
            {
                exceptionStack.AppendFormat("{0}\n", ex.Message);
            }

            lock (Locker)
            {
                exceptionStack.Append(StackTrace);
                Console.WriteLine(exceptionStack);
            }

            return exceptionStack.ToString();
        }
    }

    public class MyException : Exception
    {
        public override string StackTrace => "new StackTrace!!!!!!";
    }
}