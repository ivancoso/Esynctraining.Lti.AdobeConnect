CREATE TABLE [dbo].[QuestionForLikert] (
    [questionForLikertId] INT IDENTITY (1, 1) NOT NULL,
    [questionId]          INT NOT NULL,
    [answerOptionTypeId]  INT NULL,
    [allowOther]          BIT NULL,
    [pageNumber]          INT NULL,
    [isMandatory]         BIT NOT NULL,
    CONSTRAINT [PK_QuestionForLikert] PRIMARY KEY CLUSTERED ([questionForLikertId] ASC),
    CONSTRAINT [FK_QuestionForLikert_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId])
);

