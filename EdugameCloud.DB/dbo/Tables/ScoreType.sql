CREATE TABLE [dbo].[ScoreType] (
    [scoreTypeId]  INT           IDENTITY (1, 1) NOT NULL,
    [scoreType]    VARCHAR (50)  NULL,
    [dateCreated]  SMALLDATETIME CONSTRAINT [DF__ScoreType__DateC__71D1E811] DEFAULT (getdate()) NOT NULL,
    [isActive]     BIT           CONSTRAINT [DF__ScoreType__IsAct__72C60C4A] DEFAULT ((0)) NULL,
    [prefix]       VARCHAR (50)  NULL,
    [defaultValue] INT           CONSTRAINT [DF_ScoreType_defaultValue] DEFAULT ((10)) NOT NULL,
    CONSTRAINT [PK_ScoreType] PRIMARY KEY CLUSTERED ([scoreTypeId] ASC)
);



