using System;

namespace EdugameCloud.Lti.AdobeConnectCache
{
    internal interface ILog
    {
        void WriteLine(string value);

        void WriteLine(Exception ex, string tab = "");

    }

}
