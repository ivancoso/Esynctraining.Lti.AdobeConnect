CREATE TABLE [dbo].[LmsUserParameters] (
    [lmsUserParametersId] INT           IDENTITY (1, 1) NOT NULL,
    [provider]            NVARCHAR (50) NOT NULL,
    [wstoken]             NVARCHAR (50) NOT NULL,
    [course]              INT           NOT NULL,
    [domain]              NVARCHAR (50) NOT NULL,
    [acId]                NVARCHAR (10) NOT NULL,
    [lmsUserId]           INT           NULL,
    CONSTRAINT [PK_MoodleUserParameters] PRIMARY KEY CLUSTERED ([lmsUserParametersId] ASC),
    CONSTRAINT [FK_LmsUserParameters_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);

