using System;

namespace Esynctraining.AC.Provider.Entities
{
    // https://helpx.adobe.com/adobe-connect/webservices/report-quiz-interactions.html
    public class QuizInteractionItem
    {
        // Source: attribute display-seq
        public int DisplaySeq { get; set; }

        // Source: attribute transcript-id
        public long TranscriptId { get; set; }

        // Source: attribute interaction-id
        public long InteractionId { get; set; }

        // Source: attribute sco-id
        public long ScoId { get; set; }

        // Source: attribute score
        public int Score { get; set; }

        // Source: element name
        public string Name { get; set; }

        // Source: element sco-name
        public string ScoName { get; set; }

        // Source: element date-created
        public DateTime DateCreated { get; set; }

        // Source: element description
        public string Description { get; set; }

        // Source: element response
        public string Response { get; set; }

    }

}
