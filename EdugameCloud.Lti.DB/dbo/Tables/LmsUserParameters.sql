CREATE TABLE [dbo].[LmsUserParameters] (
    [lmsUserParametersId] INT            IDENTITY (1, 1) NOT NULL,
    [wstoken]             NVARCHAR (50)  NULL,
    [course]              INT            NOT NULL,
    [acId]                NVARCHAR (10)  NOT NULL,
    [lmsUserId]           INT            NULL,
    [companyLmsId]        INT            NULL,
    [courseName]          NVARCHAR (100) NULL,
    [userEmail]           NVARCHAR (50)  NULL,
    [lastLoggedIn]        NVARCHAR (25)  NULL,
    CONSTRAINT [PK_LmsUserParameters] PRIMARY KEY CLUSTERED ([lmsUserParametersId] ASC),
    CONSTRAINT [FK_LmsUserParameters_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]),
    CONSTRAINT [FK_LmsUserParameters_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);





