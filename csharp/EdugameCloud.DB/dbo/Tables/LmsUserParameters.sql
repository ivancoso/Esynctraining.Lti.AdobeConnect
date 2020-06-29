CREATE TABLE [dbo].[LmsUserParameters] (
    [lmsUserParametersId] INT             IDENTITY (1, 1) NOT NULL,
    [wstoken]             NVARCHAR (128)  NULL,
    [course]              NVARCHAR (50)   NOT NULL,
    [acId]                NVARCHAR (10)   NOT NULL,
    [lmsUserId]           INT             NULL,
    [companyLmsId]        INT             NULL,
    [courseName]          NVARCHAR (4000) NULL,
    [userEmail]           NVARCHAR (254)  NULL,
    [lastLoggedIn]        DATETIME        NULL,
    CONSTRAINT [PK_LmsUserParameters] PRIMARY KEY CLUSTERED ([lmsUserParametersId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_LmsUserParameters_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE,
    CONSTRAINT [FK_LmsUserParameters_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);









