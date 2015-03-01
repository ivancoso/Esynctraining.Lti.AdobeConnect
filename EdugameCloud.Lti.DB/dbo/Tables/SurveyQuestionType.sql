CREATE TABLE [dbo].[SurveyQuestionType] (
    [surveyQuestionTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [type]                 VARCHAR (50)  NULL,
    [createdBy]            INT           NULL,
    [dateCreated]          SMALLDATETIME CONSTRAINT [DF__SurveyQue__DateC__1F98B2C1] DEFAULT (getdate()) NOT NULL,
    [isActive]             BIT           CONSTRAINT [DF__SurveyQue__IsAct__208CD6FA] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_SurveyQuestionType] PRIMARY KEY CLUSTERED ([surveyQuestionTypeId] ASC)
);

