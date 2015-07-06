CREATE TABLE [dbo].[QuestionType] (
    [questionTypeId]          INT            IDENTITY (1, 1) NOT NULL,
    [type]                    VARCHAR (50)   NOT NULL,
    [questionTypeOrder]       INT            NULL,
    [questionTypeDescription] VARCHAR (200)  NULL,
    [instruction]             VARCHAR (500)  NULL,
    [correctText]             VARCHAR (500)  NULL,
    [incorrectMessage]        VARCHAR (500)  NULL,
    [isActive]                BIT            CONSTRAINT [DF__QuestionT__IsAct__1CBC4616] DEFAULT ((0)) NULL,
    [iconSource]              NVARCHAR (500) NULL,
    CONSTRAINT [PK_QuestionType] PRIMARY KEY CLUSTERED ([questionTypeId] ASC)
);







