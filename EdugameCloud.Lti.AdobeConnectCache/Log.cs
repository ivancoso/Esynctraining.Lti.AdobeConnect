using System;

namespace EdugameCloud.Lti.AdobeConnectCache
{
    internal sealed class Log : ILog
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public void WriteLine(Exception ex, string tab)
        {
            Console.WriteLine(tab + ex.Message);
        }

    }

}
