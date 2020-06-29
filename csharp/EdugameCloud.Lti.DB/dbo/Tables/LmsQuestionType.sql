CREATE TABLE [dbo].[LmsQuestionType] (
    [lmsQuestionTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [lmsProviderId]     INT           NOT NULL,
    [questionTypeId]    INT           NOT NULL,
    [lmsQuestionType]   NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_LmsQuestionType] PRIMARY KEY CLUSTERED ([lmsQuestionTypeId] ASC),
    CONSTRAINT [FK_LmsQuestionType_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId])
);



