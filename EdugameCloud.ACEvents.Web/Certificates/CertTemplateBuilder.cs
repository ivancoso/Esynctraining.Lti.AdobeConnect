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
    }

    
}