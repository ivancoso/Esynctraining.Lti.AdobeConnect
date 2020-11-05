using System;

namespace Esynctraining.AC.Provider.Entities
{
    public class TrainingItem
    {
        public string ScoId { get; set; }

        public ScoType Type { get; set; }

        public ScoIcon Icon { get; set; }

        public int? MaxRetries { get; set; }

        public string PermissionId { get; set; }

        //public long? TranscriptId { get; set; }

        //public int? Attempts { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string UrlPath { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }

        //public string ScoTag { get; set; }


        //public bool Completed { get; set; }

        public string TrStatus { get; set; }

    }

}
