CREATE TABLE [dbo].[User] (
    [userId]                     INT              IDENTITY (1, 1) NOT NULL,
    [companyId]                  INT              NOT NULL,
    [languageId]                 INT              NOT NULL,
    [timeZoneId]                 INT              NOT NULL,
    [userRoleId]                 INT              NOT NULL,
    [firstName]                  VARCHAR (100)    NOT NULL,
    [lastName]                   VARCHAR (100)    NULL,
    [password]                   VARCHAR (100)    NULL,
    [email]                      VARCHAR (450)    NOT NULL,
    [createdBy]                  INT              NULL,
    [modifiedBy]                 INT              NULL,
    [dateCreated]                SMALLDATETIME    CONSTRAINT [DF__User__DateCreate__6477ECF3] DEFAULT (getdate()) NOT NULL,
    [dateModified]               SMALLDATETIME    CONSTRAINT [DF__User__DateModifi__656C112C] DEFAULT (getdate()) NOT NULL,
    [status]                     SMALLINT         CONSTRAINT [DF_User_status] DEFAULT ((1)) NOT NULL,
    [sessionToken]               NVARCHAR (64)    NULL,
    [sessionTokenExpirationDate] DATETIME         NULL,
    [logoId]                     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([userId] ASC),
    CONSTRAINT [FK_User_Company] FOREIGN KEY ([companyId]) REFERENCES [dbo].[Company] ([companyId]),
    CONSTRAINT [FK_User_CreatedBy] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]),
    CONSTRAINT [FK_User_Language] FOREIGN KEY ([languageId]) REFERENCES [dbo].[Language] ([languageId]),
    CONSTRAINT [FK_User_Modified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]),
    CONSTRAINT [FK_User_TimeZone] FOREIGN KEY ([timeZoneId]) REFERENCES [dbo].[TimeZone] ([timeZoneId]),
    CONSTRAINT [FK_User_UserRole] FOREIGN KEY ([userRoleId]) REFERENCES [dbo].[UserRole] ([userRoleId])
);












GO


