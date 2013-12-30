CREATE TABLE [dbo].[CompanyThemeAdmin] (
    [companyThemeAdminId] INT          IDENTITY (1, 1) NOT NULL,
    [bannerImageId]       INT          NULL,
    [backgroundColor]     VARCHAR (50) NULL,
    [linkColor]           VARCHAR (50) NULL,
    CONSTRAINT [PK_CompanyThemeAdmin] PRIMARY KEY CLUSTERED ([companyThemeAdminId] ASC),
    CONSTRAINT [FK_CompanyThemeAdmin_Image] FOREIGN KEY ([bannerImageId]) REFERENCES [dbo].[File] ([fileId])
);



