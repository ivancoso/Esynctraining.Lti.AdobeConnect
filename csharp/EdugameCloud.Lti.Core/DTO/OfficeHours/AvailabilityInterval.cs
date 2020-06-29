using System.ComponentModel.DataAnnotations;

namespace EdugameCloud.Lti.Core.DTO.OfficeHours
{
    public class AvailabilityInterval
    {
        //Interval Start and End times are in minutes from the beginning of the day, so values are 0-1440

        [Range(0, 24*60, ErrorMessage = "Interval Start is out of range")]
        public int Start { get; set; }

        [Range(0, 24*60, ErrorMessage = "Interval End is out of range")]
        public int End { get; set; }
    }
}