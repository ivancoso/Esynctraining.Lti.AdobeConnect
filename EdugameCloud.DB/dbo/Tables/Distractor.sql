CREATE TABLE [dbo].[Distractor] (
    [distractorID]    INT              IDENTITY (1, 1) NOT NULL,
    [questionID]      INT              NULL,
    [distractor]      NVARCHAR (MAX)   NOT NULL,
    [distractorOrder] INT              NOT NULL,
    [score]           VARCHAR (50)     NULL,
    [isCorrect]       BIT              CONSTRAINT [DF__Distracto__IsCor__10566F31] DEFAULT ((0)) NULL,
    [createdBy]       INT              NULL,
    [modifiedBy]      INT              NULL,
    [dateCreated]     SMALLDATETIME    CONSTRAINT [DF_Distractor_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]    SMALLDATETIME    CONSTRAINT [DF_Distractor_dateModified] DEFAULT (getdate()) NOT NULL,
    [isActive]        BIT              CONSTRAINT [DF_Distractor_isActive] DEFAULT ((0)) NULL,
    [distractorType]  INT              NULL,
    [imageId]         UNIQUEIDENTIFIER NULL,
    [lmsAnswer]       NVARCHAR (100)   NULL,
    [lmsProviderId]   INT              NULL,
    [lmsAnswerId]     INT              NULL,
    [leftImageId] UNIQUEIDENTIFIER NULL, 
    [rightImageId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_Distractor] PRIMARY KEY CLUSTERED ([distractorID] ASC),
    CONSTRAINT [FK_Distractor_Distractor] FOREIGN KEY ([distractorID]) REFERENCES [dbo].[Distractor] ([distractorID]),
    CONSTRAINT [FK_Distractor_Image] FOREIGN KEY ([imageId]) REFERENCES [dbo].[File] ([fileId]),
    CONSTRAINT [FK_Distractor_LeftImage] FOREIGN KEY ([leftImageId]) REFERENCES [dbo].[File] ([fileId]),
    CONSTRAINT [FK_Distractor_RightImage] FOREIGN KEY ([rightImageId]) REFERENCES [dbo].[File] ([fileId]),
    CONSTRAINT [FK_Distractor_lmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]),
    CONSTRAINT [FK_Distractor_Question] FOREIGN KEY ([questionID]) REFERENCES [dbo].[Question] ([questionId]),
    CONSTRAINT [FK_Distractor_UserCreated] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]),
    CONSTRAINT [FK_Distractor_UserModified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId])
);













