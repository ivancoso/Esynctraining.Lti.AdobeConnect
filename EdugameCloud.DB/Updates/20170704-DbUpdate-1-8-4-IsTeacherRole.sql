ALTER TABLE [dbo].[LmsCompanyRoleMapping] 
	ADD [isTeacherRole] BIT NULL
GO

UPDATE [dbo].[LmsCompanyRoleMapping]
SET [isTeacherRole] = 0
GO

ALTER TABLE [dbo].[LmsCompanyRoleMapping] 
	ALTER COLUMN [isTeacherRole] BIT NOT NULL
GO
