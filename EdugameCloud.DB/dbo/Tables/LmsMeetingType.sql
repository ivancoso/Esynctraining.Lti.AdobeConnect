CREATE TABLE [dbo].[LmsMeetingType] (
	[lmsMeetingTypeId]		INT					NOT NULL	IDENTITY(1, 1),
	[lmsMeetingTypeName]	NVARCHAR(50)		NOT NULL,
	CONSTRAINT [PK_LmsMeetingType] PRIMARY KEY CLUSTERED ([lmsMeetingTypeId] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsMeetingType_lmsMeetingTypeName] ON [dbo].[LmsMeetingType] ([lmsMeetingTypeName])
GO
