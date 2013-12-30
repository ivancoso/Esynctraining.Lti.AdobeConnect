CREATE TABLE [dbo].[CompanyTheme] (
    [companyThemeId]        INT IDENTITY (1, 1) NOT NULL,
    [companyId]             INT NOT NULL,
    [companyThemeAdminId]   INT NOT NULL,
    [companyThemeLoginId]   INT NOT NULL,
    [companyThemeMeetingId] INT NOT NULL,
    CONSTRAINT [PK_CompanyTheme] PRIMARY KEY CLUSTERED ([companyThemeId] ASC),
    CONSTRAINT [FK_CompanyTheme_Company] FOREIGN KEY ([companyId]) REFERENCES [dbo].[Company] ([companyId]),
    CONSTRAINT [FK_CompanyTheme_CompanyThemeAdmin] FOREIGN KEY ([companyThemeAdminId]) REFERENCES [dbo].[CompanyThemeAdmin] ([companyThemeAdminId]),
    CONSTRAINT [FK_CompanyTheme_CompanyThemeLogin] FOREIGN KEY ([companyThemeLoginId]) REFERENCES [dbo].[CompanyThemeLogin] ([companyThemeLoginId]),
    CONSTRAINT [FK_CompanyTheme_CompanyThemeMeeting] FOREIGN KEY ([companyThemeMeetingId]) REFERENCES [dbo].[CompanyThemeMeeting] ([companyThemeMeetingId])
);

