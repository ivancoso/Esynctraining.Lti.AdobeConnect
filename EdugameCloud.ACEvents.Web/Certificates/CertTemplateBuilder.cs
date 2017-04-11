using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.ACEvents.Web.Certificates
{
    public class CertTemplateBuilder
    {
        private static readonly Dictionary<string, Guid> _stateCertMap = new Dictionary<string, Guid>();
        private static readonly Guid DefaultTemplateGuid = Guid.Parse("00157137-7522-483A-B34D-BAE970F9616D");
        static CertTemplateBuilder()
        {
            _stateCertMap.Add(StateNames.NY, Guid.Parse("32827C2B-C711-4E55-B57B-B01D1807CBEB"));
            _stateCertMap.Add(StateNames.GA, Guid.Parse("10fe750d-41ad-4c24-8f6a-99884250eb51"));
            _stateCertMap.Add(StateNames.PA, Guid.Parse("381875aa-df57-4aa6-af14-99a856f96cb2"));
            _stateCertMap.Add(StateNames.IL, Guid.Parse("134ce892-fccc-4d78-8a17-8c0cc1322253"));
            _stateCertMap.Add(StateNames.MD, Guid.Parse("2064fdb8-4c06-4084-8cb9-54269fcd5133"));
            _stateCertMap.Add(StateNames.MO, Guid.Parse("25d7faba-9be4-417d-883f-0c598647d1eb"));
            _stateCertMap.Add(StateNames.KY, Guid.Parse("1736983e-4de8-4520-ad93-d832141214f2"));
            _stateCertMap.Add(StateNames.WA, Guid.Parse("475dad9a-c7eb-4053-b9a0-b75f0035f2a2"));
            _stateCertMap.Add(StateNames.TX, Guid.Parse("43713819-615b-4fd7-b12e-035e8b30c68d"));
            _stateCertMap.Add(StateNames.SC, Guid.Parse("40c3b666-5b21-4fda-ad32-5017b4ae75a1"));
            _stateCertMap.Add(StateNames.NV, Guid.Parse("28a7e44a-8b36-4bc1-9d79-d3b13e0030d0"));
        }

        public static Guid GetTemplateGuid(string state)
        {
            state = state.ToLower();
            var hasValue = _stateCertMap.ContainsKey(state);
            if (hasValue)
                return _stateCertMap[state];
            return DefaultTemplateGuid;
        }
    }

    public static class StateNames
    {
        public const string NY = "ny";
        public const string MD = "md";
        public const string GA = "ga";
        public const string MO = "mo";
        public const string KY = "ky";
        public const string IL = "il";
        public const string PA = "pa";
        public const string WA = "wa";
        public const string TX = "tx";
        public const string SC = "sc";
        public const string NV = "nv";
    }

    
}