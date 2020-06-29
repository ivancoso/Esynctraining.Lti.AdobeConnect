CREATE TABLE [dbo].[AcCachePrincipal]
(
	[acCachePrincipalId]	INT		NOT NULL	IDENTITY (1, 1),

	[lmsCompanyId]			INT		NOT NULL,

	[accountId]		NVARCHAR(512)		NULL,
	[displayId]		NVARCHAR(512)		NULL,
	[email]			NVARCHAR(512)		NULL,
	[firstName]		NVARCHAR(512)		NULL,
	[hasChildren]	BIT					NULL,
	[isHidden]		BIT					NULL,
	[isPrimary]		BIT					NULL,
    [lastName]		NVARCHAR(512)		NULL,
	[login]			NVARCHAR(512)		NULL,
	[name]			NVARCHAR(512)		NULL,
	[principalId]	NVARCHAR(512)		NULL,
	[type]			NVARCHAR(512)		NULL,

	CONSTRAINT [PK_AcCachePrincipal] PRIMARY KEY CLUSTERED ([acCachePrincipalId] ASC),
	CONSTRAINT [FK_AcCachePrincipal_CompanyLms] FOREIGN KEY ([lmsCompanyId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE
)
