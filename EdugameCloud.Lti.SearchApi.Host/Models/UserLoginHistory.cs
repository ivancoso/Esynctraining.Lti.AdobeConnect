using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class UserLoginHistory
    {
        public int UserLoginHistoryId { get; set; }
        public string FromIp { get; set; }
        public int UserId { get; set; }
        public string Application { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual User User { get; set; }
    }
}
