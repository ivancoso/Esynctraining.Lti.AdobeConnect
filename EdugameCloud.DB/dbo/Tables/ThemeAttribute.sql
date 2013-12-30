CREATE TABLE [dbo].[ThemeAttribute] (
    [themeAttributeId]         INT           NOT NULL,
    [themeId]                  INT           NOT NULL,
    [themeOrder]               INT           NULL,
    [bgColor]                  CHAR (6)      NOT NULL,
    [titleColor]               CHAR (6)      NULL,
    [categoryColor]            CHAR (6)      NULL,
    [selectionColor]           CHAR (6)      NULL,
    [questionHintColor]        CHAR (6)      NULL,
    [questionTextColor]        CHAR (6)      NULL,
    [questionInstructionColor] CHAR (6)      NULL,
    [responseCorrectColor]     CHAR (6)      NULL,
    [responseIncorrectColor]   CHAR (6)      NULL,
    [distractorTextColor]      CHAR (6)      NULL,
    [createdBy]                INT           NULL,
    [modifiedBy]               INT           NULL,
    [dateCreated]              SMALLDATETIME CONSTRAINT [DF_ThemeAttribute_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]             SMALLDATETIME CONSTRAINT [DF_ThemeAttribute_dateModified] DEFAULT (getdate()) NOT NULL,
    [isActive]                 BIT           CONSTRAINT [DF_ThemeAttribute_isActive] DEFAULT ((0)) NULL,
    CONSTRAINT [FK_ThemeAttribute_Theme] FOREIGN KEY ([themeId]) REFERENCES [dbo].[Theme] ([themeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ThemeAttribute_UserCreated] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]),
    CONSTRAINT [FK_ThemeAttribute_UserModified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId])
);



