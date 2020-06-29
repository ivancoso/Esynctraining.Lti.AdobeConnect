CREATE TABLE [dbo].[SurveyQuestionHistory] (
    [surveyQuestionHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [surveyQuestionId]        INT           NOT NULL,
    [surveyId]                INT           NOT NULL,
    [surveyQuestionTypeId]    INT           NOT NULL,
    [question]                VARCHAR (MAX) NOT NULL,
    [questionOrder]           INT           NOT NULL,
    [instruction]             VARCHAR (MAX) NULL,
    [createdBy]               INT           NULL,
    [modifiedBy]              INT           NULL,
    [dateCreated]             SMALLDATETIME CONSTRAINT [DF_SurveyQuestionHistory_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]            SMALLDATETIME CONSTRAINT [DF_SurveyQuestionHistory_dateModified] DEFAULT (getdate()) NOT NULL,
    [isActive]                BIT           CONSTRAINT [DF_SurveyQuestionHistory_isActive] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_SurveyQuestionHistory] PRIMARY KEY CLUSTERED ([surveyQuestionHistoryId] ASC),
    CONSTRAINT [FK_SurveyQuestionHistory_SurveyQuestion] FOREIGN KEY ([surveyQuestionId]) REFERENCES [dbo].[SurveyQuestion] ([surveyQuestionId])
);

