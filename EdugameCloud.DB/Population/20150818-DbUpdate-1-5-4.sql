ALTER TABLE [LmsCourseMeeting] 
	ADD [reused] BIT NULL;
GO

ALTER TABLE [LmsCourseMeeting] 
	ADD [sourceCourseMeetingId]	INT	NULL
GO
