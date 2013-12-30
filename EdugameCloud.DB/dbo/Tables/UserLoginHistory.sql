CREATE TABLE [dbo].[UserLoginHistory] (
    [userLoginHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [fromIP]             VARCHAR (50)  NULL,
    [userId]             INT           NOT NULL,
    [application]        VARCHAR (255) NULL,
    [dateCreated]        DATETIME      NOT NULL,
    CONSTRAINT [PK_ContactLoginHistory] PRIMARY KEY CLUSTERED ([userLoginHistoryId] ASC),
    CONSTRAINT [FK_UserLoginHistory_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId])
);



