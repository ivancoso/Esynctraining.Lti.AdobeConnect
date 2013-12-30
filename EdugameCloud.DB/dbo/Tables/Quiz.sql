CREATE TABLE [dbo].[Quiz] (
    [quizId]          INT           IDENTITY (1, 1) NOT NULL,
    [subModuleItemId] INT           NULL,
    [quizFormatId]    INT           NULL,
    [scoreTypeId]     INT           NULL,
    [quizName]        VARCHAR (50)  NOT NULL,
    [description]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK_Quiz] PRIMARY KEY CLUSTERED ([quizId] ASC),
    CONSTRAINT [FK_Quiz_QuizFormat] FOREIGN KEY ([quizFormatId]) REFERENCES [dbo].[QuizFormat] ([quizFormatId]),
    CONSTRAINT [FK_Quiz_ScoreType] FOREIGN KEY ([scoreTypeId]) REFERENCES [dbo].[ScoreType] ([scoreTypeId]),
    CONSTRAINT [FK_Quiz_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId])
);

