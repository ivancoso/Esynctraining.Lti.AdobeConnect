CREATE TABLE [dbo].[LmsUser] (
    [lmsUserId]        INT            IDENTITY (1, 1) NOT NULL,
    [companyLmsId]     INT            NOT NULL,
    [userId]           NVARCHAR (50)  NOT NULL,
    [username]         NVARCHAR (50)  NULL,
    [password]         NVARCHAR (50)  NULL,
    [token]            NVARCHAR (100) NULL,
    [acConnectionMode] INT            CONSTRAINT [DF_LmsUser_acConnectionMode] DEFAULT ((0)) NULL,
    [primaryColor]     NVARCHAR (50)  NULL,
    [principalId]      NVARCHAR (30)  NULL,
    [name]             NVARCHAR (100) NULL,
    [email]            NVARCHAR (100) NULL,
    [userIdExtended]   NVARCHAR (50)  NULL,
    CONSTRAINT [PK_LmsUser] PRIMARY KEY CLUSTERED ([lmsUserId] ASC),
    CONSTRAINT [FK_LmsUser_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE
);















