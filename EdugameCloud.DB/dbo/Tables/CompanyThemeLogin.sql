CREATE TABLE [dbo].[CompanyThemeLogin] (
    [companyThemeLoginId] INT          IDENTITY (1, 1) NOT NULL,
    [logoImageId]         INT          NULL,
    [meetingTitle]        VARCHAR (50) NULL,
    [loginText]           VARCHAR (50) NULL,
    [backgroundColor]     VARCHAR (50) NULL,
    CONSTRAINT [PK_CompanyThemeLogin] PRIMARY KEY CLUSTERED ([companyThemeLoginId] ASC),
    CONSTRAINT [FK_CompanyThemeLogin_Image] FOREIGN KEY ([logoImageId]) REFERENCES [dbo].[File] ([fileId])
);



