CREATE TABLE [dbo].[SurveyDistractor] (
    [surveyDistractorId] INT           IDENTITY (1, 1) NOT NULL,
    [surveyQuestionId]   INT           NULL,
    [surveyDistractor]   VARCHAR (MAX) NOT NULL,
    [meta]               VARCHAR (MAX) NOT NULL,
    [answerNumber]       INT           NOT NULL,
    [isCorrect]          BIT           CONSTRAINT [DF__SurveyDis__IsCor__160F4887] DEFAULT ((0)) NULL,
    [createdBy]          INT           NULL,
    [modifiedBy]         INT           NULL,
    [dateCreated]        SMALLDATETIME CONSTRAINT [DF__SurveyDis__DateC__17036CC0] DEFAULT (getdate()) NOT NULL,
    [dateModified]       SMALLDATETIME CONSTRAINT [DF__SurveyDis__DateM__17F790F9] DEFAULT (getdate()) NOT NULL,
    [isActive]           BIT           CONSTRAINT [DF__SurveyDis__IsAct__18EBB532] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_SurveyDistractor] PRIMARY KEY CLUSTERED ([surveyDistractorId] ASC),
    CONSTRAINT [FK_SurveyDistractor_SurveyQuestion] FOREIGN KEY ([surveyQuestionId]) REFERENCES [dbo].[SurveyQuestion] ([surveyQuestionId])
);

