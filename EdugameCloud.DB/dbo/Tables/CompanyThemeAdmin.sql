CREATE TABLE [dbo].[CompanyThemeAdmin] (
    [companyThemeAdminId] INT              IDENTITY (1, 1) NOT NULL,
    [backgroundColor]     VARCHAR (50)     NULL,
    [linkColor]           VARCHAR (50)     NULL,
    [bannerImageId]       UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_CompanyThemeAdmin] PRIMARY KEY CLUSTERED ([companyThemeAdminId] ASC),
    CONSTRAINT [FK_CompanyThemeAdmin_Image] FOREIGN KEY ([bannerImageId]) REFERENCES [dbo].[File] ([fileId])
);





