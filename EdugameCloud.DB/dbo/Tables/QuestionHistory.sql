CREATE TABLE [dbo].[QuestionHistory] (
    [questionHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [questionId]        INT           NOT NULL,
    [questionTypeId]    INT           NOT NULL,
    [subModuleItemId]   INT           NULL,
    [imageId]           INT           NULL,
    [question]          VARCHAR (50)  NOT NULL,
    [questionOrder]     INT           NOT NULL,
    [instruction]       VARCHAR (MAX) NULL,
    [incorrectMessage]  VARCHAR (MAX) NULL,
    [hint]              VARCHAR (MAX) NULL,
    [createdBy]         INT           NULL,
    [modifiedBy]        INT           NULL,
    [dateCreated]       SMALLDATETIME CONSTRAINT [DF_QuestionHistory_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]      SMALLDATETIME CONSTRAINT [DF_QuestionHistory_dateModified] DEFAULT (getdate()) NOT NULL,
    [isActive]          BIT           CONSTRAINT [DF_QuestionHistory_isActive] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_QuestionHistory] PRIMARY KEY CLUSTERED ([questionHistoryId] ASC),
    CONSTRAINT [FK_QuestionHistory_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId])
);

