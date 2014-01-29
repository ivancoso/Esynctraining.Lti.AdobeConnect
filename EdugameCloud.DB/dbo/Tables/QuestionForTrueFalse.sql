CREATE TABLE [dbo].[QuestionForTrueFalse] (
    [questionForTrueFalseId] INT IDENTITY (1, 1) NOT NULL,
    [questionId]             INT NOT NULL,
    [pageNumber]             INT NULL,
    [isMandatory]            BIT NOT NULL,
    CONSTRAINT [PK_QuestionForTrueFalse] PRIMARY KEY CLUSTERED ([questionForTrueFalseId] ASC),
    CONSTRAINT [FK_QuestionForTrueFalse_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId])
);

