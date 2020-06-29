CREATE TABLE [dbo].[Test] (
    [testId]                 INT             IDENTITY (1, 1) NOT NULL,
    [subModuleItemId]        INT             NULL,
    [scoreTypeId]            INT             NULL,
    [testName]               NVARCHAR (50)   NOT NULL,
    [description]            NVARCHAR (MAX)  NULL,
    [passingScore]           DECIMAL (18, 9) NULL,
    [timeLimit]              INT             NULL,
    [instructionTitle]       NVARCHAR (MAX)  NULL,
    [instructionDescription] NVARCHAR (MAX)  NULL,
    [scoreFormat]            VARCHAR (50)    NULL,
    CONSTRAINT [PK_Test] PRIMARY KEY CLUSTERED ([testId] ASC),
    CONSTRAINT [FK_Test_ScoreType] FOREIGN KEY ([scoreTypeId]) REFERENCES [dbo].[ScoreType] ([scoreTypeId]),
    CONSTRAINT [FK_Test_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId])
);





