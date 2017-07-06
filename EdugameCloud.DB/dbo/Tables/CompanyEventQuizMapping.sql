CREATE TABLE [dbo].[CompanyEventQuizMapping] (
    [CompanyEventQuizMappingId] INT              IDENTITY (1, 1) NOT NULL,
    [PreQuizId]                 INT              NULL,
    [PostQuizId]                INT              NULL,
    [CompanyAcDomainId]         INT              NOT NULL,
    [AcEventScoId]              NVARCHAR (50)    NULL,
    [Guid]                      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_CompanyEventQuizMapping] PRIMARY KEY CLUSTERED ([CompanyEventQuizMappingId] ASC),
    CONSTRAINT [FK_CompanyEventQuizMapping_CompanyAcDomains] FOREIGN KEY ([CompanyAcDomainId]) REFERENCES [dbo].[CompanyAcDomains] ([CompanyAcServerId]),
    CONSTRAINT [FK_CompanyEventQuizMapping_Quiz] FOREIGN KEY ([PreQuizId]) REFERENCES [dbo].[Quiz] ([quizId]),
    CONSTRAINT [FK_CompanyEventQuizMapping_Quiz_Cascade] FOREIGN KEY ([PostQuizId]) REFERENCES [dbo].[Quiz] ([quizId]) ON DELETE CASCADE
);



