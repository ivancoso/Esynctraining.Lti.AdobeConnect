CREATE TABLE [dbo].[UserActivation] (
    [userActivationId] INT           IDENTITY (1, 1) NOT NULL,
    [userId]           INT           NOT NULL,
    [activationCode]   VARCHAR (50)  NOT NULL,
    [dateExpires]      SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_UserActivation] PRIMARY KEY CLUSTERED ([userActivationId] ASC),
    CONSTRAINT [FK_UserActivation_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId])
);

