CREATE TABLE [dbo].[Distractor] (
    [distractorId]    INT           IDENTITY (1, 1) NOT NULL,
    [questionId]      INT           NULL,
    [imageId]         INT           NULL,
    [distractor]      VARCHAR (MAX) NOT NULL,
    [distractorOrder] INT           NOT NULL,
    [score]           VARCHAR (50)  NULL,
    [isCorrect]       BIT           CONSTRAINT [DF__Distracto__IsCor__10566F31] DEFAULT ((0)) NULL,
    [createdBy]       INT           NULL,
    [modifiedBy]      INT           NULL,
    [dateCreated]     SMALLDATETIME CONSTRAINT [DF_Distractor_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]    SMALLDATETIME CONSTRAINT [DF_Distractor_dateModified] DEFAULT (getdate()) NOT NULL,
    [isActive]        BIT           CONSTRAINT [DF_Distractor_isActive] DEFAULT ((0)) NULL,
    [distractorType]  INT           NULL,
    CONSTRAINT [PK_Distractor] PRIMARY KEY CLUSTERED ([distractorId] ASC),
    CONSTRAINT [FK_Distractor_Image] FOREIGN KEY ([imageId]) REFERENCES [dbo].[File] ([fileId]),
    CONSTRAINT [FK_Distractor_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]),
    CONSTRAINT [FK_Distractor_UserCreated] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]),
    CONSTRAINT [FK_Distractor_UserModified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId])
);







