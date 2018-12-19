using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO.OfficeHours;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API
{
    public class OfficeHoursService
    {
        //private readonly ZoomDbContext _context;
        //private readonly INotificationService _notificationService;
        private readonly OfficeHoursModel _ohModel;
        private readonly OfficeHoursTeacherAvailabilityModel _availabilityModel;
        private readonly OfficeHoursSlotModel _slotModel;

        public OfficeHoursService(OfficeHoursModel ohModel, OfficeHoursTeacherAvailabilityModel availabilityModel, OfficeHoursSlotModel slotModel)
        {
            _ohModel = ohModel;
            _availabilityModel = availabilityModel;
            _slotModel = slotModel;
        }

        public async Task<IEnumerable<OfficeHoursTeacherAvailabilityDto>> GetAvailabilities(int ohId, int? lmsUserId)
        {
            var availabilities =
                _availabilityModel.GetAvailabilities(ohId);
            return availabilities.Select(ConvertToDto);
        }

        public async Task<IEnumerable<SlotDto>> ValidateSlotsRange(OfficeHours oh, int lmsUserId,
            OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var currentSlots = await GetSlots(oh.Id, lmsUserId, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
            var slotsToCheck =
                GetSlotsForAvailability(availabilityDto, DateTime.UtcNow, DateTime.UtcNow.AddYears(1), lmsUserId, null);
            var overlappingSlots = currentSlots.Where(cs =>
                slotsToCheck.Any(
                    x => (cs.Start <= x.Start && cs.End > x.Start) || (cs.Start < x.End && cs.End >= x.End)));
            //slotsToCheck.Any(x => currentSlots.Any(cs => (checkEmptySlots || cs.Status != 0) && ((cs.Start<= x.Start && cs.End > x.Start) || (cs.Start < x.End && cs.End >= x.End)))))
            //return false;
            return overlappingSlots;
        }

        public async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> AddAvailability(OfficeHours oh, LmsUser lmsUser, OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var overlappingSlots = await ValidateSlotsRange(oh, lmsUser.Id, availabilityDto);
            if (overlappingSlots.Any())
            {
                return OperationResultWithData<OfficeHoursTeacherAvailabilityDto>.Error(
                    "The range of dates overlaps another date range. Please choose another date range.");
            }
            var entity = ConvertFromDto(oh, lmsUser, availabilityDto);
            _availabilityModel.RegisterSave(entity, true);
            return ConvertToDto(entity).ToSuccessResult();
        }

        public async Task<IEnumerable<SlotDto>> GetSlots(int ohId, int lmsUserId, DateTime dateStart, DateTime? end = null)
        {
            var dateEnd = end.GetValueOrDefault(dateStart.AddDays(1));
            var availabilities = _availabilityModel.GetAvailabilities(ohId);//await _context.OhTeacherAvailabilities.Where(x => x.Meeting.Id == meetingId).ToListAsync(); // &&x.lmsUserId == lmsUserId
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

        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> AddSlots(int ohId, LmsUser lmsUser, IEnumerable<CreateSlotDto> dtos, OfficeHoursSlotStatus status = OfficeHoursSlotStatus.Booked)
        {
            var availabilities = _availabilityModel.GetAvailabilities(ohId); //await _context.OhTeacherAvailabilities.Include(x => x.Meeting).Where(x => x.Meeting.Id == meetingId).ToListAsync();// &&x.lmsUserId == lmsUserId
            var entities = new List<OfficeHoursSlot>();
            foreach (var dto in dtos)
            {
                //validate interval and non-busy
                var availability = availabilities.FirstOrDefault(x =>
                {
                    var availabilityDto = ConvertToDto(x);
                    var slots = GetSlotsForAvailability(availabilityDto, dto.Start, dto.End, lmsUser.Id, x);
                    return slots.Any(s => s.Start == dto.Start && s.Status == (int)OfficeHoursSlotStatus.Free);
                });
                if (availability == null)
                {
                    return OperationResultWithData<IEnumerable<SlotDto>>.Error(
                        "Time is already booked or out of any availability range. Please refresh page.");
                }

                var entity = new OfficeHoursSlot
                {
                    Availability = availability,
                    //RequesterName = requesterName,
                    User = lmsUser,
                    Start = dto.Start,
                    End = dto.End,
                    Subject = dto.Subject,
                    Questions = dto.Questions,
                    Status = (int)status
                };

                _slotModel.RegisterSave(entity, false);
                entities.Add(entity);
            }

            _slotModel.Flush();
            return entities.Select(x => ConvertToDto(x, lmsUser.Id)).ToSuccessResult();
        }

        public SlotDto ConvertToDto(OfficeHoursSlot entity, int lmsUserId)
        {
            var result = new SlotDto
            {
                Id = entity.Id,
                Status = entity.Status,
                Start = entity.Start,
                End = entity.End
            };
            if (entity.User.Id == lmsUserId || entity.Availability.User.Id == lmsUserId)
            {
                result.CanEdit = true;
                result.UserName = entity.User.Name;
                result.Subject = entity.Subject;
                result.Questions = entity.Questions;
            }

            return result;
        }

        public async Task<OperationResultWithData<SlotDto>> UpdateSlotStatus(int slotId, OfficeHoursSlotStatus status, LmsUser lmsUser, string message)
        {
            var slot = _slotModel.GetOneById(slotId).Value;
            if (slot == null)
                return OperationResultWithData<SlotDto>.Error("Slot not found");

            slot.Status = (int)status;
            _slotModel.RegisterSave(slot, true);
            if (!string.IsNullOrEmpty(message))
            {
                //var meeting = slot.Availability.Meeting;
                //var details = await _meetingService.GetMeetingApiDetails(meeting);
                //await _notificationService.SendOHCancellationEmail(slot.Start, details.Topic, message, slot.RequesterName);
            }
            return ConvertToDto(slot, lmsUser.Id).ToSuccessResult();
        }

        private async Task<OperationResultWithData<SlotDto>> UpdateSlotStatusInternal(int slotId, OfficeHoursSlotStatus status, LmsUser lmsUser)
        {
            var slot = _slotModel.GetOneById(slotId).Value;
            if (slot == null)
                return OperationResultWithData<SlotDto>.Error("Slot not found");

            slot.Status = (int)status;
            _slotModel.RegisterSave(slot, true);
            return ConvertToDto(slot, lmsUser.Id).ToSuccessResult();
        }

        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> DeleteSlots(int ohId, DenyDateDto dto,
            LmsUser lmsUser)
        {
            var dbSlots = _slotModel.GetSlotsForDate(dto.Start, dto.End, ohId);

            foreach (var dbSlot in dbSlots)
            {
                dbSlot.Status = (int)OfficeHoursSlotStatus.Cancelled;
                _slotModel.RegisterSave(dbSlot, false);

                if (!string.IsNullOrEmpty(dto.Message))
                {
                    //var meeting = dbSlot.Availability.Meeting;
                    //var details = await _meetingService.GetMeetingApiDetails(meeting);
                    //await _notificationService.SendOHCancellationEmail(dbSlot.Start, details.Topic, dto.Message, dbSlot.RequesterName);
                }
            }

            _slotModel.Flush();

            var freeSlots = (await GetSlots(ohId, lmsUser.Id, dto.Start, dto.End)).Where(x => x.Status == (int)OfficeHoursSlotStatus.Free);

            List<SlotDto> result = new List<SlotDto>();
            result.AddRange(dbSlots.Select(x => ConvertToDto(x, lmsUser.Id)));
            var resultDto = await AddSlots(ohId, lmsUser, freeSlots, OfficeHoursSlotStatus.Cancelled);
            result.AddRange(resultDto.Data);
            return (result as IEnumerable<SlotDto>).ToSuccessResult();
        }

        public async Task<OperationResultWithData<SlotDto>> DenySlotByDate(int ohId, DateTime start, LmsUser lmsUser)
        {
            var dbSlot = _slotModel.GetSlotForDate(start, ohId);

            if (dbSlot != null)
            {
                dbSlot.Status = (int)OfficeHoursSlotStatus.Cancelled;
                _slotModel.RegisterSave(dbSlot, true);
                return ConvertToDto(dbSlot, lmsUser.Id).ToSuccessResult();
            }

            var slots = await GetSlots(ohId, lmsUser.Id, start, start.AddMinutes(30)); // 30 - max time of slots
            var slot = slots.FirstOrDefault(x => x.Start == start);
            if (slot == null)
            {
                return OperationResultWithData<SlotDto>.Error(
                    "Time of selected slot is out of any availability range.");
            }

            //slot.Subject = "";
            var resultDto = await AddSlots(ohId, lmsUser, new[] { slot }, OfficeHoursSlotStatus.Cancelled);
            return resultDto.IsSuccess ? resultDto.Data.First().ToSuccessResult() : OperationResultWithData<SlotDto>.Error(resultDto.Message);
        }

        public async Task<OperationResult> DeleteSlot(int slotId, LmsUser lmsUser, string message = null)
        {
            var slot = _slotModel.GetOneById(slotId).Value;
            if (slot == null)
                return OperationResult.Error("Slot not found");

            if (slot.Status == (int)OfficeHoursSlotStatus.Cancelled)
            {
                return OperationResult.Error("Slot was deleted by teacher. Please refresh page");
            }

            if (slot.User.Id != lmsUser.Id && slot.Availability.User.Id != lmsUser.Id)
            {
                return OperationResult.Error("You don't have permissions to delete slot");
            }

            //store availability to check who cancels
            //var meeting = slot.Availability.Meeting;
            slot.Availability.Slots.Remove(slot);
            _slotModel.RegisterDelete(slot, true);
            if (!string.IsNullOrEmpty(message))
            {
                //var details = await _meetingService.GetMeetingApiDetails(meeting);
                //await _notificationService.SendOHCancellationEmail(slot.Start, details.Topic, message, slot.RequesterName);
            }

            return OperationResult.Success();
        }

        public async Task<OperationResultWithData<List<SlotDto>>> RescheduleDate(int ohId, LmsUser lmsUser,
            RescheduleDateDto dto)
        {
            List<SlotDto> result = new List<SlotDto>();
            var availabilities = _availabilityModel.GetAvailabilities(ohId);
            var availabilitiesToAdd = new List<OfficeHoursTeacherAvailabilityDto>();
            var slotsToAdd = new List<SlotDto>();
            var overlappingFreeSlots = new List<SlotDto>();
            OfficeHours oh = null;
            foreach (var availability in availabilities)
            {
                var availabilityDto = ConvertToDto(availability);
                var slots = GetSlotsForAvailability(availabilityDto, dto.Start, dto.End, lmsUser.Id, availability);
                if (slots.Any())
                {
                    var newAvailabilityDto =
                        BuildAvailabilityBySlots(slots, dto.FirstSlotTimeshift, availability.Duration);
                    oh = oh ?? availability.Meeting;
                    var overlappingSlots = await ValidateSlotsRange(oh, lmsUser.Id, newAvailabilityDto);
                    if (overlappingSlots.Any(x => x.Status != (int)OfficeHoursSlotStatus.Free))
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
                var a = await AddAvailability(oh, lmsUser, availabilityDto);
                if (!a.IsSuccess)
                {
                    //todo: delete already added availabilities
                    return OperationResultWithData<List<SlotDto>>.Error(a.Message);
                }
            }
            var notAddedSlots = new List<string>();
            var deniedSlots = new List<SlotDto>();
            //var details = await _meetingService.GetMeetingApiDetails(meeting);
            //if (dto.KeepRegistration)
            //{
            var dbSlots = _slotModel.GetSlotsForDate(dto.Start, dto.End, ohId);
            foreach (var slotDto in slotsToAdd) //todo: add many slots at once
            {
                if (slotDto.Status == (int)OfficeHoursSlotStatus.Booked)
                {
                    slotDto.Start = slotDto.Start.AddMilliseconds(dto.FirstSlotTimeshift);
                    slotDto.End = slotDto.End.AddMilliseconds(dto.FirstSlotTimeshift);
                    if (dto.KeepRegistration)
                    {
                        var s = await AddSlots(oh.Id, dbSlots.First(x => x.Id == slotDto.Id).User, new[] { slotDto });
                        if (!s.IsSuccess)
                        {
                            notAddedSlots.Add(slotDto.UserName);
                            slotDto.Id = 0;
                            slotDto.Status = (int)OfficeHoursSlotStatus.Free;
                            slotDto.Subject = null;
                            slotDto.Questions = null;
                            slotDto.UserName = null;
                            slotDto.CanEdit = false;
                        }
                        if (s.IsSuccess && !string.IsNullOrEmpty(dto.Message))
                        {
                            //await _notificationService.SendOHRescheduleEmail(slotDto.Start, s.Data.First(),
                            //    details.Topic, dto.Message);
                        }
                    }
                    else
                    {
                        //await _notificationService.SendOHCancellationEmail(slotDto.Start, details.Topic, dto.Message, slotDto.UserName);
                    }
                    var updateResult = await UpdateSlotStatusInternal(slotDto.Id, OfficeHoursSlotStatus.Cancelled, lmsUser);
                    result.Add(slotDto);
                }

                //
                else
                {
                    deniedSlots.Add(slotDto);
                }
            }
            //}
            var denyFreeSlotsResult = await AddSlots(oh.Id, lmsUser, deniedSlots, OfficeHoursSlotStatus.Cancelled);
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

        public async Task<OperationResultWithData<SlotDto>> RescheduleSlot(int slotId, LmsUser lmsUser, RescheduleSlotDto dto)
        {
            if (dto.Start < DateTime.UtcNow)
            {
                return OperationResultWithData<SlotDto>.Error("Upcoming slots couldn't be moved for elapsed time");
            }

            var slot = _slotModel.GetOneById(slotId).Value;
            //var slot = await _context.OhSlots.Include(x => x.Availability).ThenInclude(x => x.Meeting).FirstOrDefaultAsync(x => x.Id == slotId);
            if (slot == null)
                return OperationResultWithData<SlotDto>.Error("Slot not found");

            var currentSlots = await GetSlots(slot.Availability.Meeting.Id, lmsUser.Id, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));

            var busySlot = currentSlots.FirstOrDefault(x => x.Start == dto.Start);
            if (busySlot != null)
            {
                if (busySlot.Status == (int)OfficeHoursSlotStatus.Free) //free
                {
                    if ((busySlot.End - busySlot.Start).Minutes != slot.Availability.Duration)
                    {
                        return OperationResultWithData<SlotDto>.Error(
                            "Another slot with different duration is available at the same time.");
                    }

                    var slotResult = await AddSlots(slot.Availability.Meeting.Id, slot.User, new[] {
                        new CreateSlotDto
                        {
                            Start = dto.Start,
                            End = dto.Start.AddMinutes(slot.Availability.Duration),
                            Questions = slot.Questions,
                            Subject = slot.Subject
                        }});

                    if (slotResult.IsSuccess)
                    {
                        var oldSlotResult = await DeleteSlot(slotId, lmsUser);
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
            var availabilityResult = await AddAvailability(slot.Availability.Meeting, lmsUser, availabilityDto);
            if (!availabilityResult.IsSuccess)
            {
                return OperationResultWithData<SlotDto>.Error($"Error during slot time move. {availabilityResult.Message}");
            }
            var newSlotResult = await AddSlots(slot.Availability.Meeting.Id, slot.User, new[] {
                new CreateSlotDto
                {
                    Start = dto.Start,
                    End = dto.Start.AddMinutes(slot.Availability.Duration),
                    Questions = slot.Questions,
                    Subject = slot.Subject
                }});

            if (newSlotResult.IsSuccess && !string.IsNullOrEmpty(dto.Message))
            {
                //var meeting = slot.Availability.Meeting;
                //var details = await _meetingService.GetMeetingApiDetails(meeting);
                //await _notificationService.SendOHRescheduleEmail(slot.Start, newSlotResult.Data.First(), details.Topic, dto.Message);
            }
            var oldSlotResultAdd = await DeleteSlot(slotId, lmsUser);
            //todo: send email

            return newSlotResult.IsSuccess ? newSlotResult.Data.First().ToSuccessResult() : OperationResultWithData<SlotDto>.Error(newSlotResult.Message);
        }

        public async Task<OperationResult> ResetDeniedSlots(int meetingId, DenyDateDto dto)
        {
            var dbSlots = _slotModel.GetSlotsForDate(dto.Start, dto.End, meetingId).Where(x => x.Status == 2);
            foreach (var dbSlot in dbSlots)
            {
                _slotModel.RegisterDelete(dbSlot, false);
            }

            _slotModel.Flush();
            return OperationResult.Success();
        }

        private static OfficeHoursTeacherAvailability ConvertFromDto(OfficeHours oh, LmsUser lmsUser, OfficeHoursTeacherAvailabilityDto availabilityDto)
        {
            var entity = new OfficeHoursTeacherAvailability
            {
                User = lmsUser,
                Duration = availabilityDto.Duration,
                Intervals = string.Join(",", availabilityDto.Intervals.Select(x => $"{x.Start}-{x.End}")),
                DaysOfWeek = string.Join(",", availabilityDto.DaysOfWeek),
                PeriodStart = availabilityDto.PeriodStart,
                PeriodEnd = availabilityDto.PeriodEnd,
                Meeting = oh
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

            availability.DaysOfWeek = new[] { (int)availability.PeriodStart.DayOfWeek }; //assuming that we move only one day
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
            DateTime dateStart, DateTime dateEnd, int lmsUserId, OfficeHoursTeacherAvailability availability)
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

                        if (intervalCheckStart < checkStart || intervalCheckStart >= checkEnd || intervalCheckStart >= availabilityDto.PeriodEnd) //check if interval start is inside of period that we need to check
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