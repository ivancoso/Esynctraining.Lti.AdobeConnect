CREATE TABLE [dbo].[LmsUserSession] (
    [lmsUserSessionId] UNIQUEIDENTIFIER NOT NULL,
    [sessionData]      NTEXT            NULL,
    [dateCreated]      DATETIME         NOT NULL,
    [dateModified]     DATETIME         NULL,
    [companyLmsId]     INT              NOT NULL,
    [lmsUserId]        INT              NULL,
    [lmsCourseId]      NVARCHAR (50)    NOT NULL,
    CONSTRAINT [PK_LmsUserSession] PRIMARY KEY CLUSTERED ([lmsUserSessionId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_LmsUserSession_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE,
    CONSTRAINT [FK_LmsUserSession_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);


