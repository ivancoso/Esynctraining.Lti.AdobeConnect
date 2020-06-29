CREATE TABLE [dbo].[LmsCompanySetting] (
    [lmsCompanySettingId] INT            IDENTITY (1, 1) NOT NULL,
    [lmsCompanyId]        INT            NOT NULL,
    [name]                NVARCHAR(100)		NOT NULL,
    [value]               NVARCHAR(MAX)		NOT NULL,
    CONSTRAINT [PK_LmsCompanySetting] PRIMARY KEY CLUSTERED ([lmsCompanySettingId] ASC),
    CONSTRAINT [FK_LmsCompanySetting_LmsCompany] FOREIGN KEY ([lmsCompanyId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCompanySetting_lmsCompanyId_name] ON [dbo].[LmsCompanySetting] ([lmsCompanyId], [name])
GO
