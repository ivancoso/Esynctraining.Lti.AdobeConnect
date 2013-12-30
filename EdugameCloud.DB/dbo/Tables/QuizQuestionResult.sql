CREATE TABLE [dbo].[QuizQuestionResult] (
    [quizQuestionResultId] INT         IDENTITY (1, 1) NOT NULL,
    [quizResultId]         INT         NOT NULL,
    [questionId]           INT         NOT NULL,
    [question]             NCHAR (500) NOT NULL,
    [questionTypeId]       INT         NOT NULL,
    [isCorrect]            BIT         NOT NULL,
    CONSTRAINT [PK_QuizQuestionResult] PRIMARY KEY CLUSTERED ([quizQuestionResultId] ASC),
    CONSTRAINT [FK_QuizQuestionResult_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]),
    CONSTRAINT [FK_QuizQuestionResult_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]),
    CONSTRAINT [FK_QuizQuestionResult_QuizResult] FOREIGN KEY ([quizResultId]) REFERENCES [dbo].[QuizResult] ([quizResultId])
);





