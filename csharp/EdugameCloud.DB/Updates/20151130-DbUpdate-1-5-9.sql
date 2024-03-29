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


CREATE TABLE [dbo].[LmsCourseMeetingRecording] (
	[lmsCourseMeetingRecordingId]		INT				NOT NULL	IDENTITY(1, 1),
	[lmsCourseMeetingId]				INT				NOT NULL,
	[scoId]								NVARCHAR(50)	NOT NULL,

	CONSTRAINT [PK_LmsCourseMeetingRecording] PRIMARY KEY CLUSTERED ([lmsCourseMeetingRecordingId] ASC),
	CONSTRAINT [FK_LmsCourseMeetingRecording_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCourseMeetingRecording_lmsCourseMeetingId_[scoId] ON [dbo].[LmsCourseMeetingRecording] ([lmsCourseMeetingId], [scoId])
GO
