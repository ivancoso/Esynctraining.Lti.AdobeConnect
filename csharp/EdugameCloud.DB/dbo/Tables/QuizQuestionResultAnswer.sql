CREATE TABLE [dbo].[QuizQuestionResultAnswer] (
    [quizQuestionResultAnswerId] INT            IDENTITY (1, 1) NOT NULL,
    [quizQuestionResultId]       INT            NOT NULL,
    [value]                      NVARCHAR (500) NOT NULL,
    [quizDistractorAnswerId]     INT            NULL,
    CONSTRAINT [PK_QuizQuestionResultAnswer] PRIMARY KEY CLUSTERED ([quizQuestionResultAnswerId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_QuizQuestionResultAnswer_DistractorAnswer] FOREIGN KEY ([quizDistractorAnswerId]) REFERENCES [dbo].[Distractor] ([distractorId]),
    CONSTRAINT [FK_QuizQuestionResultAnswer_QuizQuestionResult] FOREIGN KEY ([quizQuestionResultId]) REFERENCES [dbo].[QuizQuestionResult] ([quizQuestionResultId])
);

