CREATE TABLE [dbo].[SurveyDistractorHistory] (
    [surveyDistractorHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [surveyDistractorId]        INT           NOT NULL,
    [surveyQuestionId]          INT           NULL,
    [surveyDistractor]          VARCHAR (MAX) NOT NULL,
    [meta]                      VARCHAR (MAX) NOT NULL,
    [answerNumber]              INT           NOT NULL,
    [isCorrect]                 BIT           CONSTRAINT [DF_SurveyDistractorHistory_isCorrect] DEFAULT ((0)) NULL,
    [createdBy]                 INT           NULL,
    [modifiedBy]                INT           NULL,
    [dateCreated]               SMALLDATETIME CONSTRAINT [DF_SurveyDistractorHistory_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]              SMALLDATETIME CONSTRAINT [DF_SurveyDistractorHistory_dateModified] DEFAULT (getdate()) NOT NULL,
    [isActive]                  BIT           CONSTRAINT [DF_SurveyDistractorHistory_isActive] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_SurveyDistractorHistory] PRIMARY KEY CLUSTERED ([surveyDistractorHistoryId] ASC),
    CONSTRAINT [FK_SurveyDistractorHistory_SurveyDistractor] FOREIGN KEY ([surveyDistractorId]) REFERENCES [dbo].[SurveyDistractor] ([surveyDistractorId])
);

