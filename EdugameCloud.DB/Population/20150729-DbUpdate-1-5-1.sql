ALTER TABLE [dbo].[ACUserMode]
	ALTER COLUMN [userMode] VARCHAR (50) NOT NULL
GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_ACUserMode_userMode]
    ON [dbo].[ACUserMode]([userMode] ASC);
GO

ALTER TABLE [dbo].[Country]
	ALTER COLUMN [countryCode]  VARCHAR (3)     NOT NULL
GO
ALTER TABLE [dbo].[Country]
	ALTER COLUMN [countryCode3] VARCHAR (4)     NOT NULL
GO
ALTER TABLE [dbo].[Country]
	ALTER COLUMN [country]      NVARCHAR (255)  NOT NULL
GO

ALTER TABLE [dbo].[EmailHistory]
	ADD [status] INT NULL
GO
UPDATE [EmailHistory]
	SET [status] = 1 -- sent
GO
ALTER TABLE [dbo].[EmailHistory]
	ALTER COLUMN [status] INT NOT NULL
GO

ALTER TABLE [dbo].[Language]
    ALTER COLUMN [language]   NVARCHAR (100) NOT NULL
GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_Language_language]
    ON [dbo].[Language]([language] ASC)
GO

ALTER TABLE [dbo].[LmsCompanySetting]
    ALTER COLUMN [value] NVARCHAR (MAX) NOT NULL
GO

ALTER TABLE [dbo].[LmsCourseMeeting]
    ALTER COLUMN [companyLmsId] INT NOT NULL
GO
ALTER TABLE [dbo].[LmsCourseMeeting]
    ALTER COLUMN [lmsMeetingTypeId] INT NOT NULL
GO

ALTER TABLE [dbo].[LmsCourseMeeting]  WITH CHECK ADD  CONSTRAINT [FK_LmsCourseMeeting_CompanyLms] FOREIGN KEY([companyLmsId])
REFERENCES [dbo].[CompanyLms] ([companyLmsId])
GO
ALTER TABLE [dbo].[LmsCourseMeeting] CHECK CONSTRAINT [FK_LmsCourseMeeting_CompanyLms]
GO
ALTER TABLE [dbo].[LmsCourseMeeting]  WITH CHECK ADD  CONSTRAINT [FK_LmsCourseMeeting_LmsMeetingType] FOREIGN KEY([lmsMeetingTypeId])
REFERENCES [dbo].[LmsMeetingType] ([lmsMeetingTypeId])
GO
ALTER TABLE [dbo].[LmsCourseMeeting] CHECK CONSTRAINT [FK_LmsCourseMeeting_LmsMeetingType]
GO
ALTER TABLE [dbo].[LmsCourseMeeting]  WITH CHECK ADD  CONSTRAINT [FK_LmsCourseMeeting_LmsUser] FOREIGN KEY([ownerId])
REFERENCES [dbo].[LmsUser] ([lmsUserId])
GO
ALTER TABLE [dbo].[LmsCourseMeeting] CHECK CONSTRAINT [FK_LmsCourseMeeting_LmsUser]
GO
ALTER TABLE [dbo].[LmsCourseMeeting]  WITH CHECK ADD  CONSTRAINT [FK_LmsCourseMeeting_OfficeHours] FOREIGN KEY([officeHoursId])
REFERENCES [dbo].[OfficeHours] ([officeHoursId])
GO
ALTER TABLE [dbo].[LmsCourseMeeting] CHECK CONSTRAINT [FK_LmsCourseMeeting_OfficeHours]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsMeetingType_lmsMeetingTypeName]
    ON [dbo].[LmsMeetingType]([lmsMeetingTypeName] ASC);
GO

ALTER TABLE [dbo].[LmsProvider]
    ALTER COLUMN [lmsProvider]      NVARCHAR (50)  NOT NULL
GO
ALTER TABLE [dbo].[LmsProvider]
    ALTER COLUMN [shortName]        NVARCHAR (50)  NOT NULL
GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsProvider_lmsProvider]
    ON [dbo].[LmsProvider]([lmsProvider] ASC);
GO

ALTER TABLE [dbo].[OfficeHours]  WITH CHECK ADD  CONSTRAINT [FK_OfficeHours_LmsUser] FOREIGN KEY ([lmsUserId]) 
REFERENCES [dbo].[LmsUser] ([lmsUserId])
GO
ALTER TABLE [dbo].[OfficeHours] CHECK CONSTRAINT [FK_OfficeHours_LmsUser]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_OfficeHours_lmsUserId]
    ON [dbo].[OfficeHours]([lmsUserId] ASC);
GO

ALTER TABLE [dbo].[QuestionType]
    ALTER COLUMN [type] VARCHAR(50) NOT NULL
GO

ALTER TABLE [dbo].[State]
    ALTER COLUMN [stateName] NVARCHAR (50)   NOT NULL
GO

ALTER TABLE [dbo].[TimeZone]
    ALTER COLUMN [timeZone]        VARCHAR (50) NOT NULL
GO
ALTER TABLE [dbo].[TimeZone]
    ALTER COLUMN [timeZoneGMTDiff] FLOAT (53)   NOT NULL
GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_TimeZone_timeZone]
    ON [dbo].[TimeZone]([timeZone] ASC);
GO
