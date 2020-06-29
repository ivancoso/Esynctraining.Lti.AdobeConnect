using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class UserActivation
    {
        public int UserActivationId { get; set; }
        public int UserId { get; set; }
        public string ActivationCode { get; set; }
        public DateTime DateExpires { get; set; }

        public virtual User User { get; set; }
    }
}
