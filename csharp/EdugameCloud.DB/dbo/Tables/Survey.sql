CREATE TABLE [dbo].[Survey] (
    [surveyId]             INT            IDENTITY (1, 1) NOT NULL,
    [subModuleItemId]      INT            NULL,
    [surveyName]           NVARCHAR (255) NOT NULL,
    [description]          NVARCHAR (MAX) NULL,
    [surveyGroupingTypeId] INT            NOT NULL,
    [lmsSurveyId]          INT            NULL,
    [lmsProviderId]        INT            NULL,
    CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED ([surveyId] ASC),
    CONSTRAINT [FK_Survey_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]),
    CONSTRAINT [FK_Survey_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]),
    CONSTRAINT [FK_Survey_SurveyGroupingType] FOREIGN KEY ([surveyGroupingTypeId]) REFERENCES [dbo].[SurveyGroupingType] ([surveyGroupingTypeId])
);







