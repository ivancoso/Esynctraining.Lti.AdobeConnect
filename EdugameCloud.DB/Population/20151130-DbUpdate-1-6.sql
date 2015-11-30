SET IDENTITY_INSERT [dbo].[Language] ON

GO

INSERT [dbo].[Language] ([languageId], [language]) VALUES(10, 'Spanish')

SET IDENTITY_INSERT [dbo].[Language] OFF

GO

ALTER TABLE [Language]
	ADD twoLetterCode CHAR(2) NULL
GO

-- https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
UPDATE [Language] SET twoLetterCode = 'en' WHERE [languageId] = 5
UPDATE [Language] SET twoLetterCode = 'es' WHERE [languageId] = 10

ALTER TABLE [Language]
	ALTER COLUMN twoLetterCode CHAR(2) NOT NULL
GO
