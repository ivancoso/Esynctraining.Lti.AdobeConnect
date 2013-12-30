CREATE TABLE [dbo].[TestQuestionResult] (
    [testQuestionResultId] INT         IDENTITY (1, 1) NOT NULL,
    [testResultId]         INT         NOT NULL,
    [questionId]           INT         NOT NULL,
    [question]             NCHAR (500) NOT NULL,
    [questionTypeId]       INT         NOT NULL,
    [isCorrect]            BIT         NOT NULL,
    CONSTRAINT [PK_TestQuestionResult] PRIMARY KEY CLUSTERED ([testQuestionResultId] ASC),
    CONSTRAINT [FK_TestQuestionResult_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]),
    CONSTRAINT [FK_TestQuestionResult_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]),
    CONSTRAINT [FK_TestQuestionResult_TestResult] FOREIGN KEY ([testResultId]) REFERENCES [dbo].[TestResult] ([testResultId])
);

