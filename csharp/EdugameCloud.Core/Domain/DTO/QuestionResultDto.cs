using System.Collections.Generic;

namespace EdugameCloud.Core.Domain.DTO
{
    public class QuestionResultDto
    {
        public int QuestionId { get; set; }

        public bool IsCorrect { get; set; }

        public IEnumerable<int> DistractorIds { get; set; }

        public string Answer { get; set; }
    }
}