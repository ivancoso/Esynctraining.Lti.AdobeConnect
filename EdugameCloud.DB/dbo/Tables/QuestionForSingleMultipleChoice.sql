CREATE TABLE [dbo].[QuestionForSingleMultipleChoice] (
    [questionForSingleMultipleChoiceId] INT IDENTITY (1, 1) NOT NULL,
    [questionId]                        INT NOT NULL,
    [allowOther]                        BIT NULL,
    [pageNumber]                        INT NULL,
    [isMandatory]                       BIT NOT NULL,
    CONSTRAINT [PK_QuestionForSingleMultipleChoice] PRIMARY KEY CLUSTERED ([questionForSingleMultipleChoiceId] ASC),
    CONSTRAINT [FK_QuestionForSingleMultipleChoice_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId])
);

