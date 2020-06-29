using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SubscriptionHistoryLog
    {
        public int SubscriptionHistoryLogId { get; set; }
        public string SubscriptionTag { get; set; }
        public DateTime? LastQueryTime { get; set; }
        public int? SubscriptionId { get; set; }
    }
}
