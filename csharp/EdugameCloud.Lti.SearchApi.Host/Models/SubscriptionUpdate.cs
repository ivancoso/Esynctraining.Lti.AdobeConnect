using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SubscriptionUpdate
    {
        public int SubscriptionUpdateId { get; set; }
        public int SubscriptionId { get; set; }
        public string Object { get; set; }
        public string ObjectId { get; set; }
        public string ChangedAspect { get; set; }
        public string Time { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
