CREATE TABLE [dbo].[LmsCompanyRoleMapping] (
	[lmsCompanyRoleMappingId]	INT				NOT NULL	IDENTITY (1, 1),
	[lmsCompanyId]			INT				NOT NULL,
	[lmsRoleName]			NVARCHAR(100)	NOT NULL,
	[isDefaultLmsRole]		BIT				NOT NULL,
	[acRole]				INT				NOT NULL,
	CONSTRAINT [PK_LmsCompanyRoleMapping] PRIMARY KEY CLUSTERED ([lmsCompanyRoleMappingId] ASC),
	CONSTRAINT [FK_LmsCompanyRoleMapping_LmsCompany] FOREIGN KEY ([lmsCompanyId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE,
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCompanyRoleMapping_lmsCompanyId_lmsRoleName] ON [dbo].[LmsCompanyRoleMapping] ([lmsCompanyId], [lmsRoleName])
GO
