using System;
using Esynctraining.Lti.Lms.Canvas.DTOs;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Canvas.Convertors
{
    public static class CanvasCalendarConvertor
    {
        public static LmsCalendarEventDTO ConvertToLmsCalendarEvent(CanvasCalendarEventDTO dto)
        {
            return new LmsCalendarEventDTO
            {
                Id = dto.id.ToString(),
                StartAt = DateTime.Parse(dto.start_at),
                EndAt = DateTime.Parse(dto.end_at),
                Description = dto.description,
                Title = dto.title
            };
        }
    }
}
