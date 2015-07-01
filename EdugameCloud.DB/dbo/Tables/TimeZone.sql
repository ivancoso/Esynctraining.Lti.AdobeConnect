CREATE TABLE [dbo].[TimeZone] (
	[timeZoneId]			INT				NOT NULL	IDENTITY(1, 1),
	[timeZone]				VARCHAR(50)		NOT NULL,
	[timeZoneGMTDiff]		FLOAT(53)		NOT NULL,
	CONSTRAINT [PK_TimeZone] PRIMARY KEY CLUSTERED ([timeZoneId] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_TimeZone_timeZone] ON [dbo].[TimeZone] ([timeZone])
GO
