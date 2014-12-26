CREATE TABLE [dbo].[SurveyResult] (
    [surveyResultId]      INT            IDENTITY (1, 1) NOT NULL,
    [surveyId]            INT            NOT NULL,
    [acSessionId]         INT            NOT NULL,
    [participantName]     NVARCHAR (50)  NOT NULL,
    [score]               INT            NOT NULL,
    [startTime]           DATETIME       CONSTRAINT [DF_SurveyResult_dateCreated] DEFAULT (getdate()) NOT NULL,
    [endTime]             DATETIME       CONSTRAINT [DF_SurveyResult_dateModified] DEFAULT (getdate()) NOT NULL,
    [dateCreated]         DATETIME       NOT NULL,
    [isArchive]           BIT            NULL,
    [email]               NVARCHAR (500) NULL,
    [lmsUserParametersId] INT            NULL,
    CONSTRAINT [PK_SurveyResult] PRIMARY KEY CLUSTERED ([surveyResultId] ASC),
    CONSTRAINT [FK_SurveyResult_LmsUserParameters] FOREIGN KEY ([lmsUserParametersId]) REFERENCES [dbo].[LmsUserParameters] ([lmsUserParametersId]),
    CONSTRAINT [FK_SurveyResult_Survey] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId])
);











