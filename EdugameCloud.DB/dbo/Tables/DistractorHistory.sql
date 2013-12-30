CREATE TABLE [dbo].[DistractorHistory] (
    [distractoryHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [distractorId]         INT           NOT NULL,
    [questionId]           INT           NULL,
    [imageId]              INT           NULL,
    [distractor]           VARCHAR (MAX) NOT NULL,
    [distractorOrder]      INT           NOT NULL,
    [score]                VARCHAR (50)  NULL,
    [isCorrect]            BIT           CONSTRAINT [DF_DistractorHistory_isCorrect] DEFAULT ((0)) NULL,
    [createdBy]            INT           NULL,
    [modifiedBy]           INT           NULL,
    [dateCreated]          SMALLDATETIME CONSTRAINT [DF_DistractorHistory_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]         SMALLDATETIME CONSTRAINT [DF_DistractorHistory_dateModified] DEFAULT (getdate()) NOT NULL,
    [isActive]             BIT           CONSTRAINT [DF_DistractorHistory_isActive] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_DistractorHistory] PRIMARY KEY CLUSTERED ([distractoryHistoryId] ASC),
    CONSTRAINT [FK_DistractorHistory_Distractor] FOREIGN KEY ([distractorId]) REFERENCES [dbo].[Distractor] ([distractorId])
);

