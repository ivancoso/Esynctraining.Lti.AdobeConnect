CREATE TABLE [dbo].[SocialUserTokens] (
    [socialUserTokensId] INT             IDENTITY (1, 1) NOT NULL,
    [key]                NVARCHAR (255)  NULL,
    [userId]             INT             NULL,
    [token]              NVARCHAR (1000) NOT NULL,
    [secret]             NVARCHAR (1000) NULL,
    [provider]           NVARCHAR (500)  NULL,
    CONSTRAINT [PK_SocialUserTokens] PRIMARY KEY CLUSTERED ([socialUserTokensId] ASC),
    CONSTRAINT [FK_SocialUserTokens_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId])
);

