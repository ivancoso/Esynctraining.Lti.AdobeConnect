CREATE TABLE [dbo].[AppletResult] (
	[appletResultId]	INT				IDENTITY (1, 1) NOT NULL,
	[appletItemId]		INT				NOT NULL,
	[acSessionId]		INT				NOT NULL,
	[participantName]	NVARCHAR(200)	NOT NULL,
	[score]				INT				NOT NULL,
	[startTime]			DATETIME		NOT NULL,
	[endTime]			DATETIME		NOT NULL,
	[dateCreated]		SMALLDATETIME	NOT NULL	CONSTRAINT [DF_AppletResult_dateCreated] DEFAULT (getdate()),
	[isArchive]			BIT					NULL,
	[email]				NVARCHAR(500)		NULL,
	CONSTRAINT [PK_AppletResult] PRIMARY KEY CLUSTERED ([appletResultId] ASC),
	CONSTRAINT [FK_AppletResult_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]),
	CONSTRAINT [FK_AppletResult_AppletItem] FOREIGN KEY ([appletItemId]) REFERENCES [dbo].[AppletItem] ([appletItemId])
);







