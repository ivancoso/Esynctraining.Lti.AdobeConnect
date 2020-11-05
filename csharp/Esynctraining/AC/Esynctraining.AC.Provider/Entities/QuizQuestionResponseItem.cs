using System;

namespace Esynctraining.AC.Provider.Entities
{
    // https://helpx.adobe.com/adobe-connect/webservices/report-quiz-question-response.html
    public partial class QuizQuestionResponseItem
    {
        // Source: attribute principal-id
        public long PrincipalId { get; set; }

        // Source: attribute interaction-id
        public long InteractionId { get; set; }

        // Source: element user-name
        public string UserName { get; set; }

        // Source: element response
        public string Response { get; set; }

        // Source: element date-created
        public DateTime DateCreated { get; set; }

    }

}
