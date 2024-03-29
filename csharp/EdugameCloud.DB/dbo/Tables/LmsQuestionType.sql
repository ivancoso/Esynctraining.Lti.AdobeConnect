﻿CREATE TABLE [dbo].[LmsQuestionType] (
    [lmsQuestionTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [lmsProviderId]     INT           NOT NULL,
    [questionTypeId]    INT           NOT NULL,
    [lmsQuestionType]   NVARCHAR (50) NOT NULL,
    [subModuleId]       INT           NULL,
    CONSTRAINT [PK_LmsQuestionType] PRIMARY KEY CLUSTERED ([lmsQuestionTypeId] ASC),
    CONSTRAINT [FK_LmsQuestionType_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]),
    CONSTRAINT [FK_LmsQuestionType_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId])
);


