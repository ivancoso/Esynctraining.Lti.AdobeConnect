CREATE TABLE [dbo].[Company] (
	[companyId]			INT					NOT NULL	IDENTITY(1, 1),
	[companyName]		VARCHAR(50)			NOT NULL,
	[addressId]			INT						NULL,
	[status]			INT					NOT NULL	CONSTRAINT [DF_Company_isActive] DEFAULT ((0)),
	[dateCreated]		SMALLDATETIME		NOT NULL	CONSTRAINT [DF_Company_dateCreated] DEFAULT (getdate()),
	[dateModified]		SMALLDATETIME		NOT NULL	CONSTRAINT [DF_Company_dateModified] DEFAULT (getdate()),
	[primaryContactId]	INT						NULL,
	[companyThemeId]	UNIQUEIDENTIFIER		NULL,
	CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([companyId] ASC),
	CONSTRAINT [FK_Company_Address] FOREIGN KEY ([addressId]) REFERENCES [dbo].[Address] ([addressId]),
	CONSTRAINT [FK_Company_CompanyTheme] FOREIGN KEY ([companyThemeId]) REFERENCES [dbo].[CompanyTheme] ([companyThemeId]),
	CONSTRAINT [FK_Company_PrimaryContact] FOREIGN KEY ([primaryContactId]) REFERENCES [dbo].[User] ([userId])
);
GO

ALTER TABLE [dbo].[Company] NOCHECK CONSTRAINT [FK_Company_Address];
GO
