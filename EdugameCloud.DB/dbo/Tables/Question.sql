CREATE TABLE [dbo].[Question] (
    [questionId]       INT              IDENTITY (1, 1) NOT NULL,
    [questionTypeId]   INT              NOT NULL,
    [subModuleItemId]  INT              NULL,
    [question]         NVARCHAR (2000)  NOT NULL,
    [questionOrder]    INT              NOT NULL,
    [instruction]      NVARCHAR (MAX)   NULL,
    [correctMessage]   NVARCHAR (MAX)   NULL,
    [correctReference] NVARCHAR (2000)  NULL,
    [incorrectMessage] NVARCHAR (MAX)   NULL,
    [hint]             NVARCHAR (MAX)   NULL,
    [createdBy]        INT              NULL,
    [modifiedBy]       INT              NULL,
    [dateCreated]      SMALLDATETIME    CONSTRAINT [DF__Question__DateCr__0B91BA14] DEFAULT (getdate()) NOT NULL,
    [dateModified]     SMALLDATETIME    CONSTRAINT [DF__Question__DateMo__0C85DE4D] DEFAULT (getdate()) NOT NULL,
    [isActive]         BIT              CONSTRAINT [DF__Question__IsActi__0D7A0286] DEFAULT ((0)) NULL,
    [scoreValue]       INT              CONSTRAINT [DF_Question_scoreValue] DEFAULT ((0)) NOT NULL,
    [imageId]          UNIQUEIDENTIFIER NULL,
    [lmsQuestionId]    INT              NULL,
    [isMoodleSingle]   BIT              NULL,
    [lmsProviderId]    INT              NULL,
    [randomizeAnswers] BIT              NULL,
	[rows]			   INT				NULL,
    CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ([questionId] ASC),
    CONSTRAINT [FK_Question_Image] FOREIGN KEY ([imageId]) REFERENCES [dbo].[File] ([fileId]),
    CONSTRAINT [FK_Question_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]),
    CONSTRAINT [FK_Question_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]),
    CONSTRAINT [FK_Question_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId])
);

































