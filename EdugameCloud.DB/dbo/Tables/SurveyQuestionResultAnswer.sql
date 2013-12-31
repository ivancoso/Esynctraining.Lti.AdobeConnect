CREATE TABLE [dbo].[SurveyQuestionResultAnswer] (
    [surveyQuestionResultAnswerId] INT            IDENTITY (1, 1) NOT NULL,
    [surveyQuestionResultId]       INT            NOT NULL,
    [surveyDistractorId]           INT            NULL,
    [value]                        NVARCHAR (500) NOT NULL,
    [surveyDistractorAnswerId]     INT            NULL,
    CONSTRAINT [PK_SurveyQuestionResultAnswer] PRIMARY KEY CLUSTERED ([surveyQuestionResultAnswerId] ASC),
    CONSTRAINT [FK_SurveyQuestionResultAnswer_Distractor] FOREIGN KEY ([surveyDistractorId]) REFERENCES [dbo].[Distractor] ([distractorId]),
    CONSTRAINT [FK_SurveyQuestionResultAnswer_SurveyQuestionResult] FOREIGN KEY ([surveyQuestionResultId]) REFERENCES [dbo].[SurveyQuestionResult] ([surveyQuestionResultId]),
    CONSTRAINT [FK_SurveyQuestionResultAnswer_SurveyQuestionResultAnswer] FOREIGN KEY ([surveyDistractorAnswerId]) REFERENCES [dbo].[Distractor] ([distractorId])
);





