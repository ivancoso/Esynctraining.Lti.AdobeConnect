CREATE TABLE [dbo].[OfficeHours] (
	[officeHoursId]		INT				NOT NULL	IDENTITY(1, 1),
	[hours]				NVARCHAR(100)		NULL,
	[scoId]				NVARCHAR(50)	NOT NULL,
	[lmsUserId]			INT				NOT NULL,
	CONSTRAINT [PK_OfficeHours] PRIMARY KEY CLUSTERED ([officeHoursId] ASC),
	CONSTRAINT [FK_OfficeHours_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_OfficeHours_lmsUserId] ON [dbo].[OfficeHours] ([lmsUserId])
GO
