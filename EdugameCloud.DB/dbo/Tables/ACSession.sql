CREATE TABLE [dbo].[ACSession] (
    [acSessionId]     INT           IDENTITY (1, 1) NOT NULL,
    [subModuleItemId] INT           NOT NULL,
    [userId]          INT           NOT NULL,
    [acUserModeId]    INT           NOT NULL,
    [accountId]       INT           NOT NULL,
    [meetingURL]      NCHAR (500)   NOT NULL,
    [scoId]           INT           NOT NULL,
    [dateCreated]     SMALLDATETIME CONSTRAINT [DF_ACSession_dateCreated] DEFAULT (getdate()) NOT NULL,
    [languageId]      INT           CONSTRAINT [DF_ACSession_languageId] DEFAULT ((5)) NOT NULL,
    [status]          INT           CONSTRAINT [DF_ACSession_status] DEFAULT ((2)) NOT NULL,
    CONSTRAINT [PK_ACSession] PRIMARY KEY CLUSTERED ([acSessionId] ASC),
    CONSTRAINT [FK_ACSession_ACUserMode] FOREIGN KEY ([acUserModeId]) REFERENCES [dbo].[ACUserMode] ([acUserModeId]),
    CONSTRAINT [FK_ACSession_Language] FOREIGN KEY ([languageId]) REFERENCES [dbo].[Language] ([languageId]),
    CONSTRAINT [FK_ACSession_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]),
    CONSTRAINT [FK_ACSession_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId])
);













