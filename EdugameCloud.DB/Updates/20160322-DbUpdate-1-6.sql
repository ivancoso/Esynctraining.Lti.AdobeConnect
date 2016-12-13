ALTER TABLE [dbo].[LmsUserParameters]
	ALTER COLUMN [wstoken] NVARCHAR(128) NULL
GO

ALTER TABLE [dbo].[LmsUserParameters]
	ALTER COLUMN [courseName] NVARCHAR(4000) NULL
GO

ALTER TABLE [dbo].[LmsUserParameters]
	ALTER COLUMN [userEmail] NVARCHAR(254) NULL
GO


-- TRICK: EXECUTE ONCE!!!!
--UPDATE 
--[dbo].[LmsUserParameters]
--SET [lastLoggedIn] =
--	CASE [lastLoggedIn] 
--		WHEN NULL 
--		THEN NULL 
--		ELSE (CASE LEN([lastLoggedIn]) WHEN 0 THEN [lastLoggedIn] ELSE LEFT([lastLoggedIn], LEN([lastLoggedIn]) - 1) END) 
--	END
--GO

ALTER TABLE [dbo].[LmsUserParameters]
	ALTER COLUMN [lastLoggedIn] DATETIME NULL
GO
