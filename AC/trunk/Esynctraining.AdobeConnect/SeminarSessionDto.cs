using System;

namespace Esynctraining.AdobeConnect
{
    public class SeminarSessionDto
    {
        public string SeminarSessionScoId { get; set; }

        public string SeminarScoId { get; set; }

        public string Name { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }

        public int ExpectedLoad { get; set; }

        public string Summary { get; set; }
    }

}
