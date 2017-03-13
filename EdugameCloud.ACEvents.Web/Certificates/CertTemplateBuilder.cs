using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.ACEvents.Web.Certificates
{
    public class CertTemplateBuilder
    {
        private static readonly Dictionary<string, Guid> _stateCertMap = new Dictionary<string, Guid>();
        private static readonly Guid DefaultTemplateGuid = Guid.Parse("A1757137-7522-483A-B34D-BAE970F9616D");
        static CertTemplateBuilder()
        {
            _stateCertMap.Add(StateNames.Ny, Guid.Parse("EF827C2B-C711-4E55-B57B-B01D1807CBEB"));
        }

        public static Guid GetTemplateGuid(string state)
        {
            var hasValue = _stateCertMap.ContainsKey(state);
            if (hasValue)
                return _stateCertMap[state];
            return DefaultTemplateGuid;
        }
      
    }



    public static class StateNames
    {
        public const string Ny = "ny";
        public const string Md = "md";
    }

    
}