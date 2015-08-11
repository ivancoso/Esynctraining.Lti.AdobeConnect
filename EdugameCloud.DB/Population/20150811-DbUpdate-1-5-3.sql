ALTER TABLE [CompanyLms] 
	ADD [isActive] BIT NULL
GO

UPDATE [CompanyLms]
SET [isActive] = 1
GO

ALTER TABLE [CompanyLms] 
	ALTER COLUMN [isActive] BIT NOT NULL
GO
