CREATE TABLE [dbo].[QuizFormat] (
    [quizFormatId]   INT           IDENTITY (1, 1) NOT NULL,
    [quizFormatName] VARCHAR (50)  NULL,
    [dateCreated]    SMALLDATETIME CONSTRAINT [DF__QuizForma__DateC__75A278F5] DEFAULT (getdate()) NOT NULL,
    [isActive]       BIT           CONSTRAINT [DF__QuizForma__IsAct__76969D2E] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_QuizFormat] PRIMARY KEY CLUSTERED ([quizFormatId] ASC)
);

