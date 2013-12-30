CREATE TABLE [dbo].[TestResult] (
    [testResultId]    INT            IDENTITY (1, 1) NOT NULL,
    [testId]          INT            NOT NULL,
    [acSessionId]     INT            NOT NULL,
    [participantName] NCHAR (200)    NOT NULL,
    [score]           INT            NOT NULL,
    [startTime]       DATETIME       NOT NULL,
    [endTime]         DATETIME       NOT NULL,
    [dateCreated]     DATETIME       CONSTRAINT [DF_TestResult_dateCreated] DEFAULT (getdate()) NOT NULL,
    [isArchive]       BIT            NULL,
    [email]           NVARCHAR (500) NULL,
    CONSTRAINT [PK_TestResult] PRIMARY KEY CLUSTERED ([testResultId] ASC),
    CONSTRAINT [FK_TestResult_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]),
    CONSTRAINT [FK_TestResult_Test] FOREIGN KEY ([testId]) REFERENCES [dbo].[Test] ([testId])
);









