CREATE TABLE [dbo].[CompanyAcDomains]
(
	[CompanyAcServerId]	INT				NOT NULL	IDENTITY(1,1),
	[AcServer]			NVARCHAR(100)	NOT NULL,
	[Username]			NVARCHAR(50)		NULL,
	[Password]			NVARCHAR(50)		NULL,
	[IsDefault]			BIT					NULL,
	[CompanyId]			INT				NOT NULL,
	CONSTRAINT [PK_CompanyAcDomains] PRIMARY KEY CLUSTERED ([CompanyAcServerId] ASC),
	CONSTRAINT [FK_CompanyAcDomains_Company] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([companyId])
) 
GO
