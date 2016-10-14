CREATE TABLE [dbo].[LmsCalendarEvent](
	[lmsCalendarEventId] [int] IDENTITY(1,1) NOT NULL,
	[eventId] [nvarchar](50) NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[startDate] [datetime2](7) NOT NULL,
	[endDate] [datetime2](7) NOT NULL,
	[lmsCourseMeetingId] [int] NOT NULL,
	[summary] [nvarchar](200) NULL,
CONSTRAINT [PK_LmsCalendarEvent] PRIMARY KEY CLUSTERED ([lmsCalendarEventId] ASC),
CONSTRAINT [FK_LmsCalendarEvent_LmsCourseMeeting] FOREIGN KEY([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
);