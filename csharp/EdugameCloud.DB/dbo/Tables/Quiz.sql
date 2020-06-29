CREATE TABLE [dbo].[Quiz] (
    [quizId]          INT              IDENTITY (1, 1) NOT NULL,
    [subModuleItemId] INT              NULL,
    [quizFormatId]    INT              NULL,
    [scoreTypeId]     INT              NULL,
    [quizName]        NVARCHAR (100)   NOT NULL,
    [description]     NVARCHAR (MAX)   NULL,
    [lmsQuizId]       INT              NULL,
    [lmsProviderId]   INT              NULL,
    [isPostQuiz]      BIT              CONSTRAINT [Quiz_IsPostQuiz] DEFAULT ((0)) NULL,
    [passingScore]    INT              CONSTRAINT [DF_Quiz_passingScore] DEFAULT ((0)) NOT NULL,
    [guid]            UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_Quiz] PRIMARY KEY CLUSTERED ([quizId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Quiz_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]),
    CONSTRAINT [FK_Quiz_QuizFormat] FOREIGN KEY ([quizFormatId]) REFERENCES [dbo].[QuizFormat] ([quizFormatId]),
    CONSTRAINT [FK_Quiz_ScoreType] FOREIGN KEY ([scoreTypeId]) REFERENCES [dbo].[ScoreType] ([scoreTypeId]),
    CONSTRAINT [FK_Quiz_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId])
);











