ALTER TABLE [dbo].[LmsUser]
	ALTER COLUMN [username] [nvarchar](128) NULL
GO

ALTER TABLE [dbo].[LmsUser]
	ALTER COLUMN [email] [nvarchar](254) NULL
GO

ALTER TABLE [dbo].[LmsUser]
	ALTER COLUMN [userId] [nvarchar](64) NOT NULL
GO
