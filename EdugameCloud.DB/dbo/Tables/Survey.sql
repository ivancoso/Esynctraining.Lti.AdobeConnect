CREATE TABLE [dbo].[Survey] (
    [surveyId]             INT           IDENTITY (1, 1) NOT NULL,
    [subModuleItemId]      INT           NULL,
    [surveyName]           VARCHAR (255) NOT NULL,
    [description]          VARCHAR (MAX) NULL,
    [surveyGroupingTypeId] INT           NOT NULL,
    CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED ([surveyId] ASC),
    CONSTRAINT [FK_Survey_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]),
    CONSTRAINT [FK_Survey_SurveyGroupingType] FOREIGN KEY ([surveyGroupingTypeId]) REFERENCES [dbo].[SurveyGroupingType] ([surveyGroupingTypeId])
);



