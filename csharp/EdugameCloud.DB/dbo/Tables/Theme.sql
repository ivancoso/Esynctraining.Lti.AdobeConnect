CREATE TABLE [dbo].[Theme] (
    [themeId]      INT           IDENTITY (1, 1) NOT NULL,
    [themeName]    VARCHAR (50)  NOT NULL,
    [createdBy]    INT           NULL,
    [modifiedBy]   INT           NULL,
    [dateCreated]  SMALLDATETIME CONSTRAINT [DF_Theme_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified] SMALLDATETIME CONSTRAINT [DF_Theme_dateModified] DEFAULT (getdate()) NOT NULL,
    [isActive]     BIT           CONSTRAINT [DF_Theme_isActive] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Theme] PRIMARY KEY CLUSTERED ([themeId] ASC),
    CONSTRAINT [FK_Theme_UserCreated] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]),
    CONSTRAINT [FK_Theme_UserModified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId])
);



