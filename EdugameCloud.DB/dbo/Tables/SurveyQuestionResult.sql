CREATE TABLE [dbo].[SurveyQuestionResult] (
    [surveyQuestionResultId] INT            IDENTITY (1, 1) NOT NULL,
    [surveyResultId]         INT            NOT NULL,
    [questionId]             INT            NOT NULL,
    [question]               NVARCHAR (500) NOT NULL,
    [questionTypeId]         INT            NOT NULL,
    [isCorrect]              BIT            NOT NULL,
    CONSTRAINT [PK_SurveyQuestionResult] PRIMARY KEY CLUSTERED ([surveyQuestionResultId] ASC),
    CONSTRAINT [FK_SurveyQuestionResult_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]),
    CONSTRAINT [FK_SurveyQuestionResult_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]),
    CONSTRAINT [FK_SurveyQuestionResult_SurveyResult] FOREIGN KEY ([surveyResultId]) REFERENCES [dbo].[SurveyResult] ([surveyResultId])
);



