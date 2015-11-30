CREATE TABLE [dbo].[Language] (
	[languageId]	INT				NOT NULL	IDENTITY(1, 1),
	[language]		NVARCHAR(100)	NOT NULL,
	[twoLetterCode]	CHAR(2)			NOT NULL,

	CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([languageId] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_Language_language] ON [dbo].[Language] ([language])
GO
