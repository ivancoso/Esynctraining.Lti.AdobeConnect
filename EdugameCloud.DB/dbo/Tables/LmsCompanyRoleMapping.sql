CREATE TABLE [dbo].[LmsCompanyRoleMapping] (
    [lmsCompanyRoleMappingId] INT            IDENTITY (1, 1) NOT NULL,
    [lmsCompanyId]            INT            NOT NULL,
    [lmsRoleName]             NVARCHAR (100) NOT NULL,
    [acRole]                  INT            NOT NULL,
    [isDefaultLmsRole]        BIT            NOT NULL,
    [isTeacherRole]           BIT            NOT NULL,
    CONSTRAINT [PK_LmsCompanyRoleMapping] PRIMARY KEY CLUSTERED ([lmsCompanyRoleMappingId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_LmsCompanyRoleMapping_LmsCompany] FOREIGN KEY ([lmsCompanyId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE
);


GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCompanyRoleMapping_lmsCompanyId_lmsRoleName] ON [dbo].[LmsCompanyRoleMapping] ([lmsCompanyId], [lmsRoleName])
GO
