using System;

namespace Esynctraining.AC.Provider.Entities
{
    public class RoomFeature
    {
        public string FeatureId { get; set; }
        public string AccountId { get; set; }
        public DateTimeOffset? DateBegin { get; set; }
        public DateTimeOffset? DateEnd { get; set; }
        public DateTimeOffset? RecordCreated { get; set; }
    }

}
