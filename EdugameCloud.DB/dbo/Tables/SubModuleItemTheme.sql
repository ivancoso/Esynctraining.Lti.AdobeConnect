CREATE TABLE [dbo].[SubModuleItemTheme] (
    [subModuleItemThemeId] INT              IDENTITY (1, 1) NOT NULL,
    [bgColor]              VARCHAR (10)     NULL,
    [bgImageId]            UNIQUEIDENTIFIER NULL,
    [titleColor]           VARCHAR (10)     NULL,
    [questionColor]        VARCHAR (10)     NULL,
    [instructionColor]     VARCHAR (10)     NULL,
    [correctColor]         VARCHAR (10)     NULL,
    [incorrectColor]       VARCHAR (10)     NULL,
    [selectionColor]       VARCHAR (10)     NULL,
    [hintColor]            VARCHAR (10)     NULL,
    [subModuleItemId]      INT              NOT NULL,
    CONSTRAINT [PK_SubModuleItemTheme] PRIMARY KEY CLUSTERED ([subModuleItemThemeId] ASC),
    CONSTRAINT [FK_SubModuleItemTheme_File] FOREIGN KEY ([bgImageId]) REFERENCES [dbo].[File] ([fileId]),
    CONSTRAINT [FK_SubModuleItemTheme_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId])
);

