using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class QuizFormat
    {
        public QuizFormat()
        {
            Quiz = new HashSet<Quiz>();
        }

        public int QuizFormatId { get; set; }
        public string QuizFormatName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Quiz> Quiz { get; set; }
    }
}
