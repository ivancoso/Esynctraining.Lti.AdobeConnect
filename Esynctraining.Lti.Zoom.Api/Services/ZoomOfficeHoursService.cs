using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomOfficeHoursService : IOfficeHoursService
    {
        private readonly ZoomDbContext _context;
        public ZoomOfficeHoursService(ZoomDbContext context)
        {
            _context = context;
        }

        public async Task GetAvailability(){}

        public async Task AddAvailability(UserDto userDto, OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var entity = ConvertFromDto(userDto, availabilityDto);
            var result = await _context.SaveChangesAsync();
        }


        private OfficeHoursTeacherAvailability ConvertFromDto(UserDto userDto, OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var entity = new OfficeHoursTeacherAvailability
            {
                LmsId = userDto.LmsId,
                Duration = availabilityDto.Duration,
                Intervals = string.Join(",", availabilityDto.Intervals.Select(x => $"{x.Start}-{x.End}")),
                DaysOfWeek = string.Join(",", availabilityDto.DaysOfWeek),
                PeriodStart = availabilityDto.PeriodStart,
                PeriodEnd = availabilityDto.PeriodEnd
            };

            return entity;
        }
    }

    public interface IOfficeHoursService
    {
        
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string LmsId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }

    public class OfficeHoursTeacherAvailabilityDto
    {
        //public int MeetingId { get; set; }

        public List<AvailabilityInterval> Intervals { get; set; }
        public int Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek
        public int[] DaysOfWeek { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }



    public class AvailabilityInterval
    {
        public int Start { get; set; }
        public int End { get; set; }
    }
}