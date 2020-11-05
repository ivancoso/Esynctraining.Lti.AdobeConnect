namespace Esynctraining.AC.Provider.Entities
{
    // https://helpx.adobe.com/adobe-connect/webservices/report-quiz-question-distribution.html
    public class QuizQuestionDistributionItem
    {
        // Source: attribute display-seq
        public int DisplaySeq { get; set; }

        // Source: attribute interaction-id
        public long InteractionId { get; set; }

        // Source: attribute num-correct
        public int NumCorrect { get; set; }

        // Source: attribute num-incorrect
        public int NumIncorrect { get; set; }

        // Source: attribute total-responses
        public int TotalResponses { get; set; }

        // Source: attribute percentage-correct
        public int PercentageCorrect { get; set; }

        // Source: attribute score
        public int Score { get; set; }

        // Source: element name
        public string Name { get; set; }

        // Source: element description
        public string Description { get; set; }

    }

}
