using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Lti.Zoom.Common.Dto.OfficeHours;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class ZoomOfficeHoursService : IOfficeHoursService
    {
        private readonly ZoomDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ZoomMeetingApiService _meetingService;
        private readonly ILmsLicenseAccessor _licenseAccessor;

        public ZoomOfficeHoursService(ZoomDbContext context, INotificationService notificationService,
            ILmsLicenseAccessor licenseAccessor, ZoomMeetingApiService meetingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
        }

        public async Task<IEnumerable<OfficeHoursTeacherAvailabilityDto>> GetAvailabilities(int meetingId, string userId)
        {
            var licenseDto = await _licenseAccessor.GetLicense();
            //todo: return slots accross the courses
            var ohMeeting = await _context.LmsCourseMeetings.FirstOrDefaultAsync(x =>
                x.LicenseKey == licenseDto.ConsumerKey && x.ProviderHostId == userId && x.Type == 2);
            var availabilities =
                await _context.OhTeacherAvailabilities.Where(x =>
                    x.Meeting.Id == meetingId).ToListAsync(); // &&x.lmsUserId == lmsUserId
            return availabilities.Select(ConvertToDto);
        }

        public async Task<IEnumerable<SlotDto>> ValidateSlotsRange(LmsCourseMeeting meeting, string lmsUserId,
            OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var currentSlots = await GetSlots(meeting.Id, lmsUserId, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
            var slotsToCheck = 
                GetSlotsForAvailability(availabilityDto, DateTime.UtcNow, DateTime.UtcNow.AddYears(1), lmsUserId, null);
            var overlappingSlots = currentSlots.Where(cs =>
                slotsToCheck.Any(
                    x => (cs.Start <= x.Start && cs.End > x.Start) || (cs.Start < x.End && cs.End >= x.End)));
                //slotsToCheck.Any(x => currentSlots.Any(cs => (checkEmptySlots || cs.Status != 0) && ((cs.Start<= x.Start && cs.End > x.Start) || (cs.Start < x.End && cs.End >= x.End)))))
                //return false;
            return overlappingSlots;
        }

        public async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> AddAvailability(LmsCourseMeeting meeting, string lmsUserId, OfficeHoursTeacherAvailabilityDto availabilityDto)
        {


            var overlappingSlots = await ValidateSlotsRange(meeting, lmsUserId, availabilityDto);
            if (overlappingSlots.Any())
            {
                return OperationResultWithData<OfficeHoursTeacherAvailabilityDto>.Error(
                    "The range of dates overlaps another date range. Please choose another date range.");
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

        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> AddSlots(int meetingId, string lmsUserId, string requesterName, IEnumerable<CreateSlotDto> dtos, int status = 1)
        {
            var availabilities = await _context.OhTeacherAvailabilities.Include(x => x.Meeting).Where(x => x.Meeting.Id == meetingId).ToListAsync();// &&x.lmsUserId == lmsUserId
            var entities = new List<OfficeHoursSlot>();
            foreach (var dto in dtos)
            {
                //validate interval and non-busy
                var availability = availabilities.FirstOrDefault(x =>
                {
                    var availabilityDto = ConvertToDto(x);
                    var slots = GetSlotsForAvailability(availabilityDto, dto.Start, dto.End, lmsUserId, x);
                    return slots.Any(s => s.Start == dto.Start && s.Status == 0);
                });
                if (availability == null)
                {
                    return OperationResultWithData<IEnumerable<SlotDto>>.Error(
                        "Time is already booked or out of any availability range. Please refresh page.");
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
                entities.Add(entity);
            }

            var result = await _context.SaveChangesAsync();
            return entities.Select(x => ConvertToDto(x, lmsUserId)).ToSuccessResult();
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

        public async Task<OperationResultWithData<SlotDto>> UpdateSlotStatus(int slotId, int status, string lmsUserId, string message)
        {
            var slot = await _context.OhSlots.FirstOrDefaultAsync(x => x.Id == slotId);
            if (slot == null)
                return OperationResultWithData<SlotDto>.Error("Slot not found");

            slot.Status = status;
            var result = await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(message))
            {
                var meeting = slot.Availability.Meeting;
                var details = await _meetingService.GetMeetingApiDetails(meeting);
                await _notificationService.SendOHCancellationEmail(slot.Start, details.Topic, message, slot.RequesterName);
            }
            return ConvertToDto(slot, lmsUserId).ToSuccessResult();
        }

        private async Task<OperationResultWithData<SlotDto>> UpdateSlotStatusInternal(int slotId, int status, string lmsUserId)
        {
            var slot = await _context.OhSlots.FirstOrDefaultAsync(x => x.Id == slotId);
            if (slot == null)
                return OperationResultWithData<SlotDto>.Error("Slot not found");

            slot.Status = status;
            var result = await _context.SaveChangesAsync();
            return ConvertToDto(slot, lmsUserId).ToSuccessResult();
        }

        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> DeleteSlots(int meetingId, DenyDateDto dto,
            string lmsUserId)
        {
            var dbSlots =
                await _context.OhSlots.Where(x =>
                    x.Start >= dto.Start && x.Start < dto.End && x.Availability.Meeting.Id == meetingId).ToListAsync();
            foreach (var dbSlot in dbSlots)
            {
                dbSlot.Status = 2;
                if (!string.IsNullOrEmpty(dto.Message))
                {
                    var meeting = dbSlot.Availability.Meeting;
                var details = await _meetingService.GetMeetingApiDetails(meeting);
                    await _notificationService.SendOHCancellationEmail(dbSlot.Start, details.Topic, dto.Message, dbSlot.RequesterName);
                }
            }

            var saveResult = await _context.SaveChangesAsync();

            var freeSlots = (await GetSlots(meetingId, lmsUserId, dto.Start, dto.End)).Where(x => x.Status == 0);

            List<SlotDto> result = new List<SlotDto>();
            result.AddRange(dbSlots.Select(x => ConvertToDto(x, lmsUserId)));
            var addDeletedSlotsResult = await AddSlots(meetingId, lmsUserId, null, freeSlots, 2);
            if (!addDeletedSlotsResult.IsSuccess)
            {
                return OperationResultWithData<IEnumerable<SlotDto>>.Error(addDeletedSlotsResult.Message);
            }

            result.AddRange(addDeletedSlotsResult.Data);
            return (result as IEnumerable<SlotDto>).ToSuccessResult();
        }

        public async Task<OperationResultWithData<SlotDto>> DenySlotByDate(int meetingId, DateTime start, string lmsUserId)
        {
            var dbSlot =
                await _context.OhSlots.FirstOrDefaultAsync(x =>
                    x.Start == start && x.Availability.Meeting.Id == meetingId);
            if (dbSlot != null)
            {
                dbSlot.Status = 2;
                var saveResult = await _context.SaveChangesAsync();
                return ConvertToDto(dbSlot, lmsUserId).ToSuccessResult();
            }
             
            var slots = await GetSlots(meetingId, lmsUserId, start, start.AddMinutes(30)); // 30 - max time of slots
            var slot = slots.FirstOrDefault(x => x.Start == start);
            if (slot == null)
            {
                return OperationResultWithData<SlotDto>.Error(
                    "Time of selected slot is out of any availability range.");
            }

            //slot.Subject = "";
            var resultDto = await AddSlots(meetingId, lmsUserId, "", new[] {slot}, 2);
            return resultDto.IsSuccess ? resultDto.Data.First().ToSuccessResult() : OperationResultWithData<SlotDto>.Error(resultDto.Message);
        }

        public async Task<OperationResult> DeleteSlot(int slotId, string lmsUserId, string message = null)
        {
            var slot = await _context.OhSlots.FirstOrDefaultAsync(x => x.Id == slotId);
            if (slot == null)
                return OperationResult.Error("Slot not found");

            if (slot.Status == 2)
            {
                return OperationResult.Error("Slot was deleted by teacher. Please refresh page");
            }

            //store availability to check who cancels
            var meeting = slot.Availability.Meeting;
            _context.Remove(slot);
            var result = await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(message))
            {
                var details = await _meetingService.GetMeetingApiDetails(meeting);
                await _notificationService.SendOHCancellationEmail(slot.Start, details.Topic, message, slot.RequesterName);
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> ResetDeniedSlots(int meetingId, DenyDateDto dto, string lmsUserId)
        {
            var dbSlots =
                await _context.OhSlots.Where(x =>
                    x.Start >= dto.Start && x.Start < dto.End && x.Availability.Meeting.Id == meetingId && x.Status == 2).ToListAsync();
            foreach (var dbSlot in dbSlots)
            {
                _context.Remove(dbSlot);
            }

            var saveResult = await _context.SaveChangesAsync();
            return OperationResult.Success();
        }

        public async Task<OperationResultWithData<List<SlotDto>>> RescheduleDate(int meetingId, string lmsUserId,
            RescheduleDateDto dto)
        {
            List<SlotDto> result = new List<SlotDto>();
            var availabilities = await _context.OhTeacherAvailabilities.Include(x => x.Meeting)
                .Where(x => x.Meeting.Id == meetingId).ToListAsync(); // &&x.lmsUserId == lmsUserId
            var availabilitiesToAdd = new List<OfficeHoursTeacherAvailabilityDto>();
            var slotsToAdd = new List<SlotDto>();
            var overlappingFreeSlots = new List<SlotDto>();
            LmsCourseMeeting meeting = null;
            foreach (var availability in availabilities)
            {
                var availabilityDto = ConvertToDto(availability);
                var slots = GetSlotsForAvailability(availabilityDto, dto.Start, dto.End, lmsUserId, availability);
                if (slots.Any())
                {
                    var newAvailabilityDto =
                        BuildAvailabilityBySlots(slots, dto.FirstSlotTimeshift, availability.Duration);
                    meeting = meeting ?? availability.Meeting;
                    var overlappingSlots = await ValidateSlotsRange(meeting, lmsUserId, newAvailabilityDto);
                    if (overlappingSlots.Any(x => x.Status != 0))
                    {
                        return OperationResultWithData<List<SlotDto>>.Error(
                            "The range of dates overlaps another date range. Please choose another date range.");
                    }
                    if (overlappingSlots.Any(x => (x.End - x.Start).Minutes != availability.Duration))
                    {
                        return OperationResultWithData<List<SlotDto>>.Error(
                            "The duration of new slots doesn't match duration of overalpped free slots.");
                    }
                    if (overlappingSlots.Any())
                    {
                        var slotsForAvailability = slots.Where(x =>
                            overlappingSlots.All(os => os.Start != x.Start.AddMilliseconds(dto.FirstSlotTimeshift)));
                        if (slotsForAvailability.Any())
                        {
                            newAvailabilityDto = BuildAvailabilityBySlots(slotsForAvailability, dto.FirstSlotTimeshift,
                                availability.Duration);
                        }
                        else
                        {
                            slotsToAdd.AddRange(slots);
                            continue;
                        }
                    }
                    availabilitiesToAdd.Add(newAvailabilityDto);
                    slotsToAdd.AddRange(slots);
                }
            }

            foreach (var availabilityDto in availabilitiesToAdd)
            {
                var a = await AddAvailability(meeting, lmsUserId, availabilityDto);
                if (!a.IsSuccess)
                {
                    //todo: delete already added availabilities
                    return OperationResultWithData<List<SlotDto>>.Error(a.Message);
                }
            }
            var notAddedSlots = new List<string>();
            var deniedSlots = new List<SlotDto>();
            var details = await _meetingService.GetMeetingApiDetails(meeting);
            //if (dto.KeepRegistration)
            //{
            var dbSlots =
                await _context.OhSlots.Where(x =>
                    x.Start >= dto.Start && x.Start < dto.End && x.Availability.Meeting.Id == meetingId).ToListAsync();
            foreach (var slotDto in slotsToAdd) //todo: add many slots at once
            {
                if (slotDto.Status == 1)
                {
                    slotDto.Start = slotDto.Start.AddMilliseconds(dto.FirstSlotTimeshift);
                    slotDto.End = slotDto.End.AddMilliseconds(dto.FirstSlotTimeshift);
                    if (dto.KeepRegistration)
                    {
                        var s = await AddSlots(meeting.Id, dbSlots.First(x => x.Id == slotDto.Id).LmsUserId, slotDto.UserName, new[] {slotDto});
                        if (!s.IsSuccess)
                        {
                            notAddedSlots.Add(slotDto.UserName);
                            slotDto.Id = 0;
                            slotDto.Status = 0;
                            slotDto.Subject = null;
                            slotDto.Questions = null;
                            slotDto.UserName = null;
                            slotDto.CanEdit = false;
                        }
                        if (s.IsSuccess && !string.IsNullOrEmpty(dto.Message))
                        {
                            await _notificationService.SendOHRescheduleEmail(slotDto.Start, s.Data.First(),
                                details.Topic, dto.Message);
                        }
                    }
                    else
                    {
                        await _notificationService.SendOHCancellationEmail(slotDto.Start, details.Topic, dto.Message, slotDto.UserName);
                    }
                    var updateResult = await UpdateSlotStatusInternal(slotDto.Id, 2, lmsUserId);
                    result.Add(slotDto);
                }

                //
                else
                {
                    deniedSlots.Add(slotDto);
                }
            }
            //}
            var denyFreeSlotsResult = await AddSlots(meeting.Id, lmsUserId, null, deniedSlots, 2);
            deniedSlots.ForEach(x =>
            {
                x.Start = x.Start.AddMilliseconds(dto.FirstSlotTimeshift);
                x.End = x.End.AddMilliseconds(dto.FirstSlotTimeshift);
            });

            result.AddRange(deniedSlots);
            var opResult = result.ToSuccessResult();
            if (notAddedSlots.Any())
            {
                opResult.Message = $"Slots for the following users were not moved: {String.Join(", ", notAddedSlots)}";
            }

            return opResult;
        }

        public async Task<OperationResultWithData<SlotDto>> RescheduleSlot(int slotId, string lmsUserId, RescheduleSlotDto dto)
        {
            if (dto.Start < DateTime.UtcNow)
            {
                return OperationResultWithData<SlotDto>.Error("Upcoming slots couldn't be moved for elapsed time");
            }

            var slot = await _context.OhSlots.Include(x => x.Availability).ThenInclude(x => x.Meeting).FirstOrDefaultAsync(x => x.Id == slotId);
            if (slot == null)
                return OperationResultWithData<SlotDto>.Error("Slot not found");

            var currentSlots = await GetSlots(slot.Availability.Meeting.Id, lmsUserId, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));

            var busySlot = currentSlots.FirstOrDefault(x => x.Start == dto.Start);
            if (busySlot != null)
            {
                if (busySlot.Status == 0) //free
                {
                    if ((busySlot.End - busySlot.Start).Minutes != slot.Availability.Duration)
                    {
                        return OperationResultWithData<SlotDto>.Error(
                            "Another slot with different duration is available at the same time.");
                    }

                    var slotResult = await AddSlots(slot.Availability.Meeting.Id, slot.LmsUserId, slot.RequesterName, new[] {
                    new CreateSlotDto
                    {
                        Start = dto.Start,
                        End = dto.Start.AddMinutes(slot.Availability.Duration),
                        Questions = slot.Questions,
                        Subject = slot.Subject
                    }});

                    if (slotResult.IsSuccess)
                    {
                        var oldSlotResult = await DeleteSlot(slotId, lmsUserId);
                        return slotResult.Data.First().ToSuccessResult();
                    }
                    //todo: send email
                    return OperationResultWithData<SlotDto>.Error(slotResult.Message);
                }
                else
                {
                    return OperationResultWithData<SlotDto>.Error(
                        "Another slot has already been booked at this time or time is not available for booking.");
                }
            }
            if (currentSlots.Any(cs =>
                (cs.Start <= dto.Start && cs.End > dto.Start) ||
                (cs.Start < dto.Start.AddMinutes(slot.Availability.Duration) &&
                 cs.End >= dto.Start.AddMinutes(slot.Availability.Duration))))
            {
                return OperationResultWithData<SlotDto>.Error(
                    "Another slot with different duration is available at the same time.");
            }

            var availabilityDto = new OfficeHoursTeacherAvailabilityDto
            {
                Intervals = new List<AvailabilityInterval> { new AvailabilityInterval { Start = dto.Start.Hour * 60 + dto.Start.Minute, End = dto.Start.Hour * 60 + dto.Start.Minute + slot.Availability.Duration } },
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
            var newSlotResult = await AddSlots(slot.Availability.Meeting.Id, slot.LmsUserId, slot.RequesterName, new [] {
                new CreateSlotDto
                {
                    Start = dto.Start,
                    End = dto.Start.AddMinutes(slot.Availability.Duration),
                    Questions = slot.Questions,
                    Subject = slot.Subject
                }});

            if (newSlotResult.IsSuccess && !string.IsNullOrEmpty(dto.Message))
            {
                var meeting = slot.Availability.Meeting;
                var details = await _meetingService.GetMeetingApiDetails(meeting);
                await _notificationService.SendOHRescheduleEmail(slot.Start, newSlotResult.Data.First(), details.Topic, dto.Message);
            }
            var oldSlotResultAdd = await DeleteSlot(slotId, lmsUserId);
            //todo: send email

            return newSlotResult.IsSuccess ? newSlotResult.Data.First().ToSuccessResult() : OperationResultWithData<SlotDto>.Error(newSlotResult.Message);
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

        private OfficeHoursTeacherAvailabilityDto BuildAvailabilityBySlots(IEnumerable<SlotDto> slots, long timeshift, int duration)
        {
            var orderedSlots = slots.OrderBy(x => x.Start);
            var availabilityStart = orderedSlots.First().Start;
            var availabilityEnd = orderedSlots.Last().End;
            var availability = new OfficeHoursTeacherAvailabilityDto
            {
                Duration = duration,
                PeriodEnd = availabilityEnd.AddMilliseconds(timeshift),
                PeriodStart = availabilityStart.AddMilliseconds(timeshift),
                Intervals = BuildIntervalsBySlots(orderedSlots, timeshift, duration)
            };

            availability.DaysOfWeek = new[] {(int)availability.PeriodStart.DayOfWeek}; //assuming that we move only one day
            return availability;
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

        private IEnumerable<SlotDto> GetSlotsForAvailability(OfficeHoursTeacherAvailabilityDto availabilityDto,
            DateTime dateStart, DateTime dateEnd, string lmsUserId, OfficeHoursTeacherAvailability availability)
        {
            var slots = new List<SlotDto>();
            var availabilityStart = availabilityDto.PeriodStart;
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
                            : new DateTime(checkStart.Year, checkStart.Month, checkStart.Day, checkStart.Hour, 0, 0, DateTimeKind.Utc)
                                .AddMinutes((checkStart.Minute / availabilityDto.Duration + (checkStart.Minute % availabilityDto.Duration == 0 ? 0 : 1)) * availabilityDto.Duration);

                        if (intervalCheckStart < checkStart || intervalCheckStart >= checkEnd) //check if interval start is inside of period that we need to check
                            continue;

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


        private List<AvailabilityInterval> BuildIntervalsBySlots(IEnumerable<SlotDto> slots, long timeshift, int duration)
        {
            //assuming that slots are ordered
            List<AvailabilityInterval> result = new List<AvailabilityInterval>();
            AvailabilityInterval currentInterval = null;
            foreach (var slot in slots.OrderBy(x => x.Start))
            {
                var interval = GetInterval(slot.Start, timeshift, duration);
                if (currentInterval == null)
                {
                    currentInterval = interval;
                }
                else
                {
                    if (currentInterval.End != interval.Start)
                    {
                        result.Add(currentInterval);
                        currentInterval = interval;
                    }
                    else
                    {
                        currentInterval.End = interval.End;
                    }
                }
            }

            result.Add(currentInterval);
            return result;
        }

        private AvailabilityInterval GetInterval(DateTime date, long timeshift, int duration)
        {
            var shiftedDate = date.AddMilliseconds(timeshift);
            var endOfDay = 60 * 24;
            var intervalStart = shiftedDate.Hour * 60 + shiftedDate.Minute;
            var intervalEnd = intervalStart + duration;
            return new AvailabilityInterval
            {
                Start = intervalStart,
                End = intervalEnd >= endOfDay ? intervalEnd - endOfDay : intervalEnd
            };
        }
    }
}