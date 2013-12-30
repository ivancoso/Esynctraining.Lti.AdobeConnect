CREATE TABLE [dbo].[SNLink] (
    [snLinkId]    INT             IDENTITY (1, 1) NOT NULL,
    [snProfileId] INT             NOT NULL,
    [linkName]    NVARCHAR (255)  NOT NULL,
    [linkValue]   NVARCHAR (2000) NULL,
    CONSTRAINT [PK_SNLink] PRIMARY KEY CLUSTERED ([snLinkId] ASC),
    CONSTRAINT [FK_SNLink_SNProfile] FOREIGN KEY ([snProfileId]) REFERENCES [dbo].[SNProfile] ([snProfileId])
);

