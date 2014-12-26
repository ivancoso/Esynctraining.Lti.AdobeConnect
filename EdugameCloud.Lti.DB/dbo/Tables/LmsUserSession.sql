﻿CREATE TABLE [dbo].[LmsUserSession] (
    [lmsUserSessionId] UNIQUEIDENTIFIER NOT NULL,
    [sessionData]      NTEXT            NULL,
    [dateCreated]      DATETIME         NOT NULL,
    [dateModified]     DATETIME         NULL,
    [companyLmsId]     INT              NOT NULL,
    [lmsUserId]        INT              NULL,
    [lmsCourseId]      INT              NOT NULL,
    CONSTRAINT [PK_LmsUserSession_1] PRIMARY KEY CLUSTERED ([lmsUserSessionId] ASC),
    CONSTRAINT [FK_LmsUserSession_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]),
    CONSTRAINT [FK_LmsUserSession_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);

