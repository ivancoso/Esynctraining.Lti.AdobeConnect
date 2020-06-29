using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EdugameCloud.Sakai.TestClient.Models
{
    public class SakaiParameters
    {
        public string ServiceUrl { get; set; }

        public string ConsumerKey { get; set; }

        public string SharedSecret { get; set; }


        public string lis_result_sourcedid { get; set; }


        public string OAuthSignatureMethod { get; set; }

        public string OAuthVersion { get; set; }

        public string LtiVersion { get; set; }

        public string LtiMessageType { get; set; }


        [ReadOnly(true)]
        [DataType(DataType.MultilineText)]
        public string RequestParams { get; set; }

        [ReadOnly(true)]
        [DataType(DataType.MultilineText)]
        public string ResponseString { get; set; }

        public SakaiParameters()
        {
            ServiceUrl = "https://future.update.eitdigital.eu/imsblis/service/";

            ConsumerKey = "15f6d048-d8f4-4c6a-96df-02ddf1ac4588";
            SharedSecret = "1e174153-725e-49f4-819b-95613d11c30d";

            lis_result_sourcedid = "38f3c894f23887b5118944a6217efa380588b5cb147336ab2998934360a17c18:::69197f6e-94ce-4621-99cb-4893006d198b:::bae523a5-7cf3-4c71-b558-5bafe8aa03a3";
            OAuthSignatureMethod = "HMAC-SHA1";
            OAuthVersion = "1.0";
            LtiVersion = "LTI-1p0";
            LtiMessageType = "basic-lis-readmembershipsforcontext";
        }

    }

}