CREATE TABLE [dbo].[LmsCourseMeeting] (
	[lmsCourseMeetingId]	INT				NOT NULL	IDENTITY(1, 1),
	[courseId]				INT				NOT NULL,
	[scoId]					NVARCHAR(50)	NOT NULL,
	[companyLmsId]			INT				NOT NULL,
	[lmsMeetingTypeId]		INT				NOT NULL,
	[cachedUsers]			NVARCHAR(MAX)		NULL,
	[addedToCache]			DATETIME			NULL,
	[officeHoursId]			INT					NULL,
	[ownerId]				INT					NULL,
	[meetingNameJson]		NVARCHAR(4000)		NULL,

	CONSTRAINT [PK_LmsCourseMeeting] PRIMARY KEY CLUSTERED ([lmsCourseMeetingId] ASC),
	CONSTRAINT [FK_LmsCourseMeeting_LmsMeetingType] FOREIGN KEY ([lmsMeetingTypeId]) REFERENCES [dbo].[LmsMeetingType] ([lmsMeetingTypeId]),
	CONSTRAINT [FK_LmsCourseMeeting_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId])
);
GO
