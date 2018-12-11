ALTER TABLE [LmsMeetingSession]
	ADD [lmsCalendarEventId] INT NULL

GO

ALTER TABLE [LmsCourseMeeting]
	ADD [lmsCalendarEventId] INT NULL
GO