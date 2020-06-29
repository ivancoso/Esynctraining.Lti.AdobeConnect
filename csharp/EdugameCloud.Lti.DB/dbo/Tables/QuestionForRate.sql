CREATE TABLE [dbo].[QuestionForRate] (
    [questionForRateId] INT            IDENTITY (1, 1) NOT NULL,
    [questionId]        INT            NOT NULL,
    [restrictions]      NVARCHAR (255) NULL,
    [allowOther]        BIT            NULL,
    [pageNumber]        INT            NULL,
    [isMandatory]       BIT            NOT NULL,
    CONSTRAINT [PK_QuestionForRate] PRIMARY KEY CLUSTERED ([questionForRateId] ASC),
    CONSTRAINT [FK_QuestionForRate_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId])
);

