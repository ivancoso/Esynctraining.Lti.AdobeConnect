CREATE TABLE [dbo].[LmsCourseMeeting] (
	[lmsCourseMeetingId]	INT				NOT NULL	IDENTITY(1, 1),
	[courseId]				INT				NOT NULL,
	[scoId]					NVARCHAR(50)		NULL,	-- NOTE: IS NULL for Office Hours
	[companyLmsId]			INT				NOT NULL,
	[lmsMeetingTypeId]		INT				NOT NULL,
	[officeHoursId]			INT					NULL,
	[ownerId]				INT					NULL,
	[meetingNameJson]		NVARCHAR(4000)		NULL,
	[reused]				BIT					NULL, -- TODO: not null
	[sourceCourseMeetingId]	INT					NULL,
	[audioProfileId]		NVARCHAR(50)		NULL,

	CONSTRAINT [PK_LmsCourseMeeting] PRIMARY KEY CLUSTERED ([lmsCourseMeetingId] ASC),
	CONSTRAINT [FK_LmsCourseMeeting_LmsMeetingType] FOREIGN KEY ([lmsMeetingTypeId]) REFERENCES [dbo].[LmsMeetingType] ([lmsMeetingTypeId]),
	CONSTRAINT [FK_LmsCourseMeeting_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]),
	CONSTRAINT [FK_LmsCourseMeeting_OfficeHours] FOREIGN KEY ([officeHoursId]) REFERENCES [dbo].[OfficeHours] ([officeHoursId]),
	CONSTRAINT [FK_LmsCourseMeeting_LmsUser] FOREIGN KEY ([ownerId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);
GO
