using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomOfficeHoursService : IOfficeHoursService
    {
        private readonly ZoomDbContext _context;


        public ZoomOfficeHoursService(ZoomDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<OfficeHoursTeacherAvailabilityDto> GetAvailability(int meetingId)
        {
            var availability =
                await _context.OhTeacherAvailabilities.FirstOrDefaultAsync(x =>
                    x.Meeting.Id == meetingId); // &&x.lmsUserId == lmsUserId
            return availability == null ? null : ConvertToDto(availability);
        }

        private static OfficeHoursTeacherAvailabilityDto ConvertToDto(OfficeHoursTeacherAvailability availability)
        {
            return new OfficeHoursTeacherAvailabilityDto
            {

                Duration = availability.Duration,
                PeriodStart = availability.PeriodStart,
                PeriodEnd = availability.PeriodEnd,
                DaysOfWeek = availability.DaysOfWeek.Split(',').Select(x => Int32.Parse(x)).ToArray(),
                Intervals = availability.Intervals.Split(',').Select(x =>
                {
                    var intervals = x.Split('-');
                    return new AvailabilityInterval { Start = int.Parse(intervals[0]), End = int.Parse(intervals[1]) };
                }).ToList()
            };
        }

        public async Task AddAvailability(LmsCourseMeeting meeting, string lmsUserId, OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var entity = ConvertFromDto(meeting, lmsUserId, availabilityDto);
            _context.Add(entity);
            var result = await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SlotDto>> GetSlots(int meetingId, DateTime dateStart, string lmsUserId)
        {
            var dateEnd = dateStart.AddDays(1);
            var availability = await _context.OhTeacherAvailabilities.FirstOrDefaultAsync(x => x.Meeting.Id == meetingId); // &&x.lmsUserId == lmsUserId
            var availabilityDto = ConvertToDto(availability);
            var checkStart = availabilityDto.PeriodStart > dateStart ? availabilityDto.PeriodStart : dateStart;
            var periodEnd = availabilityDto.PeriodEnd.AddDays(1);
            var checkEnd = periodEnd < dateEnd ? periodEnd : dateEnd;
            var dbSlots = _context.OhSlots.Where(x => x.Meeting.Id == meetingId);
            var slots = new List<SlotDto>();
            while (checkStart < checkEnd)
            {
                if (availabilityDto.DaysOfWeek.Contains((int)checkStart.DayOfWeek))
                {
                    //dayly cycles for intervals
                    foreach(var interval in availabilityDto.Intervals)
                    {
                        var intervalCheckStart = checkStart.AddMinutes(interval.Start);
                        var intervalCheckEnd = checkStart.AddMinutes(interval.End);
                        while (intervalCheckStart < intervalCheckEnd)
                        {
                            var dbSlot = await dbSlots.FirstOrDefaultAsync(x => x.Start == intervalCheckStart); // && end
                            slots.Add(dbSlot == null ? new SlotDto
                            {
                                Start = intervalCheckStart, End = intervalCheckStart.AddMinutes(availabilityDto.Duration)
                            } : ConvertToDto(dbSlot, lmsUserId));

                            intervalCheckStart = intervalCheckStart.AddMinutes(availabilityDto.Duration);
                        }
                    }
                }

                checkStart = checkStart.AddDays(1);
            }

            return slots;
        }

        public async Task<SlotDto> AddSlot(int meetingId, string lmsUserId, string requesterName, CreateSlotDto dto, int status = 1)
        {
            var availability = await _context.OhTeacherAvailabilities.FirstOrDefaultAsync(x => x.Meeting.Id == meetingId); // &&x.lmsUserId == lmsUserId
            //validate interval and non-busy
            var entity = new OfficeHoursSlot
            {
                Availability = availability,
                Meeting = availability.Meeting,
                RequesterName = requesterName,
                LmsUserId = lmsUserId,
                Start = dto.Start,
                End = dto.End,
                Subject = dto.Subject,
                Questions = dto.Questions,
                Status = status
            };

            _context.Add(entity);
            var result = await _context.SaveChangesAsync();
            return ConvertToDto(entity, lmsUserId);
        }

        public SlotDto ConvertToDto(OfficeHoursSlot entity, string lmsUserId)
        {
            var result = new SlotDto
            {
                Id = entity.Id,
                Status = entity.Status,
                Start = entity.Start,
                End = entity.End
            };
            if (entity.LmsUserId == lmsUserId || entity.Availability.LmsUserId == lmsUserId)
            {
                result.CanEdit = true;
                result.UserName = entity.RequesterName;
                result.Subject = entity.Subject;
                result.Questions = entity.Questions;
            }

            return result;
        }

        public async Task<OperationResult> UpdateSlotStatus(int slotId, int status)
        {
            var slot = await _context.OhSlots.FirstOrDefaultAsync(x => x.Id == slotId);
            if(slot == null)
                return OperationResult.Error("Slot not found");

            slot.Status = status;
            var result = await _context.SaveChangesAsync();
            return OperationResult.Success();
        }

        public async Task<OperationResult> CancelSlot(int slotId, string lmsUserId)
        {
            var slot = await _context.OhSlots.FirstOrDefaultAsync(x => x.Id == slotId);
            if (slot == null)
                return OperationResult.Error("Slot not found");
            //store availability to check who cancels
            _context.Remove(slot);
            var result = await _context.SaveChangesAsync();

            //send emails

            return OperationResult.Success();
        }

        private static OfficeHoursTeacherAvailability ConvertFromDto(LmsCourseMeeting meeting, string lmsUserId, OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var entity = new OfficeHoursTeacherAvailability
            {
                LmsUserId = lmsUserId,
                Duration = availabilityDto.Duration,
                Intervals = string.Join(",", availabilityDto.Intervals.Select(x => $"{x.Start}-{x.End}")),
                DaysOfWeek = string.Join(",", availabilityDto.DaysOfWeek),
                PeriodStart = availabilityDto.PeriodStart,
                PeriodEnd = availabilityDto.PeriodEnd,
                Meeting = meeting
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

    [DataContract]
    public class OfficeHoursTeacherAvailabilityDto
    {
        //public int MeetingId { get; set; }

        public List<AvailabilityInterval> Intervals { get; set; }
        public int Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek
        public int[] DaysOfWeek { get; set; }

        //[DataMember(Name="startDate")]
        public DateTime PeriodStart { get; set; }
        //[DataMember(Name="endDate")]
        public DateTime PeriodEnd { get; set; }
    }



    public class AvailabilityInterval
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class SlotDto : CreateSlotDto
    {
        public int Id { get; set; }
        public int Status { get; set; } // 0 - Free, 1 - Booked, 2 - NotAvailable
        public string UserName { get; set; }
        public bool CanEdit { get; set; }
    }

    public class CreateSlotDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string Subject { get; set; }
        public string Questions { get; set; }
    }
}