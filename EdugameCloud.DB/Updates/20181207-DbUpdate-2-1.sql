EXEC sp_rename '[LmsMeetingSession].[eventId]', 'lmsCalendarEventId', 'COLUMN'

GO

ALTER TABLE [LmsCourseMeeting]
	ADD [lmsCalendarEventId] NVARCHAR(50) NULL
GO