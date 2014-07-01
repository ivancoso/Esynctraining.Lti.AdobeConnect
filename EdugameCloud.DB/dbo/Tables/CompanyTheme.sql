CREATE TABLE [dbo].[CompanyTheme] (
    [companyThemeId]            UNIQUEIDENTIFIER NOT NULL,
    [headerBackgroundColor]     VARCHAR (10)     NULL,
    [buttonColor]               VARCHAR (10)     NULL,
    [buttonTextColor]           VARCHAR (10)     NULL,
    [gridHeaderTextColor]       VARCHAR (10)     NULL,
    [gridHeaderBackgroundColor] VARCHAR (10)     NULL,
    [gridRolloverColor]         VARCHAR (10)     NULL,
    [logoId]                    UNIQUEIDENTIFIER NULL,
    [loginHeaderTextColor]      VARCHAR (10)     NULL,
    CONSTRAINT [PK_CompanyTheme] PRIMARY KEY CLUSTERED ([companyThemeId] ASC),
    CONSTRAINT [FK_CompanyTheme_File] FOREIGN KEY ([logoId]) REFERENCES [dbo].[File] ([fileId])
);







