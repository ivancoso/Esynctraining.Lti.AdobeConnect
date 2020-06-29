CREATE TABLE [dbo].[QuestionForOpenAnswer] (
    [questionForOpenAnswerId] INT            IDENTITY (1, 1) NOT NULL,
    [questionId]              INT            NOT NULL,
    [restrictions]            NVARCHAR (255) NULL,
    [pageNumber]              INT            NULL,
    [isMandatory]             BIT            NOT NULL,
    CONSTRAINT [PK_QuestionForOpenAnswer] PRIMARY KEY CLUSTERED ([questionForOpenAnswerId] ASC),
    CONSTRAINT [FK_QuestionForOpenAnswer_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId])
);

