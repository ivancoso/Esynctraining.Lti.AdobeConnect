using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SocialUserTokens
    {
        public int SocialUserTokensId { get; set; }
        public string Key { get; set; }
        public int? UserId { get; set; }
        public string Token { get; set; }
        public string Secret { get; set; }
        public string Provider { get; set; }

        public virtual User User { get; set; }
    }
}
