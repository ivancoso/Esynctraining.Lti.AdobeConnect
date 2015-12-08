CREATE TABLE [dbo].[LmsCourseMeetingRecording] (
	[lmsCourseMeetingRecordingId]		INT				NOT NULL	IDENTITY(1, 1),
	[lmsCourseMeetingId]				INT				NOT NULL,
	[scoId]								NVARCHAR(50)	NOT NULL,

	CONSTRAINT [PK_LmsCourseMeetingRecording] PRIMARY KEY CLUSTERED ([lmsCourseMeetingRecordingId] ASC),
	CONSTRAINT [FK_LmsCourseMeetingRecording_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCourseMeetingRecording_lmsCourseMeetingId_[scoId] ON [dbo].[LmsCourseMeetingRecording] ([lmsCourseMeetingId], [scoId])
GO
