CREATE TABLE [dbo].[SurveyQuestion] (
    [surveyQuestionId]     INT           IDENTITY (1, 1) NOT NULL,
    [surveyId]             INT           NOT NULL,
    [surveyQuestionTypeId] INT           NOT NULL,
    [question]             VARCHAR (MAX) NOT NULL,
    [questionOrder]        INT           NOT NULL,
    [instruction]          VARCHAR (MAX) NULL,
    [createdBy]            INT           NULL,
    [modifiedBy]           INT           NULL,
    [dateCreated]          SMALLDATETIME CONSTRAINT [DF__SurveyQue__DateC__236943A5] DEFAULT (getdate()) NOT NULL,
    [dateModified]         SMALLDATETIME CONSTRAINT [DF__SurveyQue__DateM__245D67DE] DEFAULT (getdate()) NOT NULL,
    [isActive]             BIT           CONSTRAINT [DF__SurveyQue__IsAct__25518C17] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_SurveyQuestion] PRIMARY KEY CLUSTERED ([surveyQuestionId] ASC),
    CONSTRAINT [FK_SurveyQuestion_Survey] FOREIGN KEY ([surveyId]) REFERENCES [dbo].[Survey] ([surveyId]),
    CONSTRAINT [FK_SurveyQuestion_SurveyQuestionType] FOREIGN KEY ([surveyQuestionTypeId]) REFERENCES [dbo].[SurveyQuestionType] ([surveyQuestionTypeId])
);

