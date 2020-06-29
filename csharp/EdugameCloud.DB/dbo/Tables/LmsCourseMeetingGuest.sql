CREATE TABLE [dbo].[LmsCourseMeetingGuest] (
	[lmsCourseMeetingGuestId]		INT				NOT NULL	IDENTITY (1, 1),
	[lmsCourseMeetingId]			INT				NOT NULL,
	[principalId]					NVARCHAR(30)	NOT NULL,

	CONSTRAINT [PK_LmsCourseMeetingGuest] PRIMARY KEY CLUSTERED ([lmsCourseMeetingGuestId] ASC),
	CONSTRAINT [FK_LmsCourseMeetingGuest_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId]) ON DELETE CASCADE
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCourseMeetingGuest_lmsCourseMeetingId_principalId] ON [dbo].[LmsCourseMeetingGuest] ([lmsCourseMeetingId], [principalId])
GO
