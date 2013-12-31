CREATE TABLE [dbo].[ACUserMode] (
    [acUserModeId] INT              IDENTITY (1, 1) NOT NULL,
    [userMode]     VARCHAR (50)     NULL,
    [imageId]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ACUserMode] PRIMARY KEY CLUSTERED ([acUserModeId] ASC),
    CONSTRAINT [FK_ACUserMode_Image] FOREIGN KEY ([imageId]) REFERENCES [dbo].[File] ([fileId])
);







