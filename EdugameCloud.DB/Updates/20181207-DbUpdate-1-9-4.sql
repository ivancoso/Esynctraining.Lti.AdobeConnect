ALTER TABLE [LmsMeetingSession]
	ADD [lmsCalendarEventId] NVARCHAR(50) NULL

GO

ALTER TABLE [LmsCourseMeeting]
	ADD [lmsCalendarEventId] NVARCHAR(50) NULL
GO