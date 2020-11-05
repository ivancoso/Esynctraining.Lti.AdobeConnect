using System;

namespace Esynctraining.AC.Provider.Entities
{
    public class ReportActiveMeetingsItem
    {
        public string ScoId { get; set; }
        public int ActiveParticipants { get; set; }
        public int LengthMinutes { get; set; }
        public string Name { get; set; }
        public string UrlPath { get; set; }
        public DateTime DateBegin { get; set; }
    }
}