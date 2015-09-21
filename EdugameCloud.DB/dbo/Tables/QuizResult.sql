CREATE TABLE [dbo].[QuizResult] (
    [quizResultId]        INT            IDENTITY (1, 1) NOT NULL,
    [quizId]              INT            NOT NULL,
    [acSessionId]         INT            NOT NULL,
    [participantName]     NVARCHAR(200)  NOT NULL,
    [score]               INT            NOT NULL,
    [startTime]           DATETIME       NOT NULL,
    [endTime]             DATETIME       NOT NULL,
    [dateCreated]         DATETIME       CONSTRAINT [DF_QuizResult_dateCreated] DEFAULT (getdate()) NOT NULL,
    [isArchive]           BIT            NULL,
    [email]               NVARCHAR (500) NULL,
    [lmsId]               INT            NULL,
    [acEmail]             NVARCHAR (500) NULL,
    [isCompleted]         BIT            NULL,
    [lmsUserParametersId] INT            NULL,
    CONSTRAINT [PK_QuizResult] PRIMARY KEY CLUSTERED ([quizResultId] ASC),
    CONSTRAINT [FK_QuizResult_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]),
    CONSTRAINT [FK_QuizResult_Quiz] FOREIGN KEY ([quizId]) REFERENCES [dbo].[Quiz] ([quizId])
);













