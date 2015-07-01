CREATE TABLE [dbo].[ACUserMode] (
	[acUserModeId]		INT					NOT NULL	IDENTITY(1, 1),
	[userMode]			VARCHAR(50)			NOT NULL,
	[imageId]			UNIQUEIDENTIFIER		NULL,
	CONSTRAINT [PK_ACUserMode] PRIMARY KEY CLUSTERED ([acUserModeId] ASC),
	CONSTRAINT [FK_ACUserMode_Image] FOREIGN KEY ([imageId]) REFERENCES [dbo].[File] ([fileId])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_ACUserMode_userMode] ON [dbo].[ACUserMode] ([userMode])
GO
