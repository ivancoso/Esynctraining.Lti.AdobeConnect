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
            ServiceUrl = "https://edgesandbox.apus.edu/imsblis/service/";

            ConsumerKey = "2b4d6ae7-a4db-47ea-9933-27fb8c430440";
            SharedSecret = "54637ae3-2a12-41dc-b1e6-dd49e8cb5f9e";

            lis_result_sourcedid = "0521a34a3d5a6fc7b037af0b97557a8c18d95d1f42f02189382b1bd13c8c4ee5:::2ac9cbd7-cf18-4110-b8bc-c1c35b57145d:::content:28";

            OAuthSignatureMethod = "HMAC-SHA1";
            OAuthVersion = "1.0";
            LtiVersion = "LTI-1p0";
            LtiMessageType = "basic-lis-readmembershipsforcontext";
        }

    }

}