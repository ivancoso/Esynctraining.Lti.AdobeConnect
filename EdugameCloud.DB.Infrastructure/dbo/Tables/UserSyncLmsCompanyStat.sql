CREATE TABLE [dbo].[UserSyncLmsCompanyStat]
(
	[id]				INT			NOT NULL	IDENTITY (1, 1),
	[companyLmsId]		INT			NOT NULL,
	[startTime]			DATETIME	NOT NULL,
	[endTime]			DATETIME	NOT NULL,
	[courseCount]		INT			NOT NULL,
	[meetingCount]		INT			NOT NULL,

	CONSTRAINT [PK_UserSyncLmsCompanyStat] PRIMARY KEY CLUSTERED ([id] ASC),
)
