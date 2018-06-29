using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<OfficeHoursTeacherAvailabilityDto>> GetAvailabilities(int meetingId, int licenseId, string userId)
        {
            //todo: return slots accross the courses
            var ohMeeting = await _context.LmsCourseMeetings.FirstOrDefaultAsync(x =>
                x.LicenseId == licenseId && x.ProviderHostId == userId && x.Type == 2);
            var availabilities =
                await _context.OhTeacherAvailabilities.Where(x =>
                    x.Meeting.Id == meetingId).ToListAsync(); // &&x.lmsUserId == lmsUserId
            return availabilities.Select(ConvertToDto);
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

        public async Task<bool> ValidateSlotsRange(LmsCourseMeeting meeting, string lmsUserId,
            OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var currentSlots = await GetSlots(meeting.Id, lmsUserId, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
            var slotsToCheck = 
                GetSlotsForAvailability(availabilityDto, DateTime.UtcNow, DateTime.UtcNow.AddYears(1), lmsUserId, null);
            if(slotsToCheck.Any(x => currentSlots.Any(cs => (cs.Start<= x.Start && cs.End > x.Start) || (cs.Start < x.End && cs.End >= x.End))))
                return false;
            return true;
        }

        public async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> AddAvailability(LmsCourseMeeting meeting, string lmsUserId, OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            if (! await ValidateSlotsRange(meeting, lmsUserId, availabilityDto))
            {
                return OperationResultWithData<OfficeHoursTeacherAvailabilityDto>.Error(
                    "The range of dates crosses another date range. Please choose another date range.");
            }
            var entity = ConvertFromDto(meeting, lmsUserId, availabilityDto);
            _context.Add(entity);
            var result = await _context.SaveChangesAsync();
            return ConvertToDto(entity).ToSuccessResult();
        }

        public async Task<IEnumerable<SlotDto>> GetSlots(int meetingId, string lmsUserId, DateTime dateStart, DateTime? end = null)
        {
            var dateEnd = end.GetValueOrDefault(dateStart.AddDays(1));
            var availabilities = await _context.OhTeacherAvailabilities.Where(x => x.Meeting.Id == meetingId).ToListAsync(); // &&x.lmsUserId == lmsUserId
            var slots = new List<SlotDto>();
            foreach (var availability in availabilities)
            {
                var availabilityDto = ConvertToDto(availability);
                var slotsForAvailability =
                    GetSlotsForAvailability(availabilityDto, dateStart, dateEnd, lmsUserId, availability);
                slots.AddRange(slotsForAvailability);

            }

            return slots;
        }

        private IEnumerable<SlotDto> GetSlotsForAvailability(OfficeHoursTeacherAvailabilityDto availabilityDto, 
            DateTime dateStart, DateTime dateEnd, string lmsUserId, OfficeHoursTeacherAvailability availability)
        {
            var slots = new List<SlotDto>();
            var availabilityStart = availabilityDto.PeriodStart.Date;
            var checkStart = availabilityStart > dateStart ? availabilityStart : dateStart;
            var periodEnd = availabilityDto.PeriodEnd.Date.AddDays(1);
            var checkEnd = periodEnd < dateEnd ? periodEnd : dateEnd;

            var dayCheckStart = checkStart.Date;
            var dayCheckEnd = checkEnd.Date.AddDays(1);
            var dbSlots = availability?.Slots;

            while (dayCheckStart < dayCheckEnd)
            {
                if (availabilityDto.DaysOfWeek.Contains((int)dayCheckStart.DayOfWeek))
                {
                    //dayly cycles for intervals
                    foreach (var interval in availabilityDto.Intervals)
                    {
                        var intervalCheckStart = dayCheckStart.AddMinutes(interval.Start) > checkStart
                            ? dayCheckStart.AddMinutes(interval.Start)
                            : checkStart;
                        var intervalCheckEnd = interval.End > interval.Start
                            ? dayCheckStart.AddMinutes(interval.End)
                            : dayCheckStart.AddDays(1).AddMinutes(interval.End);
                        while (intervalCheckStart < intervalCheckEnd)
                        {
                            var dbSlot =
                                dbSlots?.FirstOrDefault(x => x.Start == intervalCheckStart); // && end
                            slots.Add(dbSlot == null
                                ? new SlotDto
                                {
                                    Start = intervalCheckStart,
                                    End = intervalCheckStart.AddMinutes(availabilityDto.Duration)
                                }
                                : ConvertToDto(dbSlot, lmsUserId));

                            intervalCheckStart = intervalCheckStart.AddMinutes(availabilityDto.Duration);
                        }
                    }
                }

                dayCheckStart = dayCheckStart.AddDays(1);
            }

            return slots;
        }

        public async Task<OperationResultWithData<SlotDto>> AddSlot(int meetingId, string lmsUserId, string requesterName, CreateSlotDto dto, int status = 1)
        {
            var availabilities = await _context.OhTeacherAvailabilities.Include(x => x.Meeting).Where(x => x.Meeting.Id == meetingId).ToListAsync();// &&x.lmsUserId == lmsUserId
            //validate interval and non-busy
            var availability = availabilities.FirstOrDefault(x =>
            {
                var availabilityDto = ConvertToDto(x);
                var slots = GetSlotsForAvailability(availabilityDto, dto.Start, dto.End, lmsUserId, x);
                return slots.Any(s => s.Start == dto.Start && s.Status == 0);
            });
            if (availability == null)
            {
                return OperationResultWithData<SlotDto>.Error(
                    "Time of selected slot is out of any availability range.");
            }

            var entity = new OfficeHoursSlot
            {
                Availability = availability,
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
            return ConvertToDto(entity, lmsUserId).ToSuccessResult();
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

        public async Task<OperationResult> DeleteSlot(int slotId, string lmsUserId)
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


        public async Task<OperationResultWithData<SlotDto>> RescheduleSlot(int slotId, string lmsUserId, RescheduleSlotDto dto)
        {
            var slot = await _context.OhSlots.Include(x => x.Availability).ThenInclude(x => x.Meeting).FirstOrDefaultAsync(x => x.Id == slotId);
            if (slot == null)
                return OperationResultWithData<SlotDto>.Error("Slot not found");
            var currentSlots = await GetSlots(slot.Availability.Meeting.Id, lmsUserId, DateTime.UtcNow,
                DateTime.UtcNow.AddYears(1));
            var busySlot = currentSlots.FirstOrDefault(x => x.Start == dto.Start);
            if (busySlot != null)
            {
                if (busySlot.Status == 0) //free
                {
                    var newSlot = await AddSlot(slot.Availability.Meeting.Id, lmsUserId, slot.RequesterName,
                        new CreateSlotDto
                        {
                            Start = dto.Start,
                            End = dto.Start.AddMinutes(slot.Availability.Duration),
                            Questions = slot.Questions,
                            Subject = slot.Subject
                        });
                    var oldSlotResult = DeleteSlot(slotId, lmsUserId);
                    //todo: send email

                    return newSlot;
                }
                else
                {
                    return OperationResultWithData<SlotDto>.Error(
                        "Another slot has already been booked at this time or time is not available for booking.");
                }
            }

                var availabilityDto = new OfficeHoursTeacherAvailabilityDto
                {
                    Intervals = new List<AvailabilityInterval> { new AvailabilityInterval { Start = dto.Start.Hour* 60 + dto.Start.Minute, End = dto.Start.Hour * 60 + dto.Start.Minute + slot.Availability.Duration} },
                    PeriodStart = dto.Start,
                    PeriodEnd = dto.Start.AddMinutes(slot.Availability.Duration),
                    Duration = slot.Availability.Duration,
                    DaysOfWeek = new[] { (int)dto.Start.DayOfWeek }
                };
            var availabilityResult = await AddAvailability(slot.Availability.Meeting, lmsUserId, availabilityDto);
            if (!availabilityResult.IsSuccess)
            {
                return OperationResultWithData<SlotDto>.Error($"Error during slot time move. {availabilityResult.Message}");
            }
            var newSlotAdd = await AddSlot(slot.Availability.Meeting.Id, lmsUserId, slot.RequesterName,
                new CreateSlotDto
                {
                    Start = dto.Start,
                    End = dto.Start.AddMinutes(slot.Availability.Duration),
                    Questions = slot.Questions,
                    Subject = slot.Subject
                });
            var oldSlotResultAdd = DeleteSlot(slotId, lmsUserId);
            //todo: send email

            return newSlotAdd;
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

    public class RescheduleSlotDto
    {
        public DateTime Start { get; set; }
        public bool SendNotification { get; set; }
        public string Message { get; set; }
    }
}