CREATE TABLE [dbo].[SurveyGroupingType] (
    [surveyGroupingTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [surveyGroupingType]   NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SurveyGroupingType] PRIMARY KEY CLUSTERED ([surveyGroupingTypeId] ASC)
);

