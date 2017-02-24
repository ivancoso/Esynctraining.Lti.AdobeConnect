alter table dbo.Company
add useEventMapping bit
GO
update dbo.Company
set useEventMapping = 0
GO
alter table dbo.Company add constraint Company_UseEventMappingDefault DEFAULT 0 FOR useEventMapping


alter table dbo.Quiz
add isPostQuiz bit
GO
update dbo.Quiz
set isPostQuiz = 0
GO
alter table dbo.Quiz add constraint Quiz_IsPostQuiz DEFAULT 0 FOR isPostQuiz


GO

CREATE TABLE [dbo].[CompanyEventQuizMapping](
	[CompanyEventQuizMappingId] [int] IDENTITY(1,1) NOT NULL,
	[PreQuizId] [int] NULL,
	[PostQuizId] [int] NULL,
	[CompanyAcDomainId] [int] NOT NULL,
	[AcEventScoId] [nvarchar](50) NULL,
 CONSTRAINT [PK_CompanyEventQuizMapping] PRIMARY KEY CLUSTERED 
(
	[CompanyEventQuizMappingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CompanyEventQuizMapping]  WITH CHECK ADD  CONSTRAINT [FK_CompanyEventQuizMapping_CompanyAcDomains] FOREIGN KEY([CompanyAcDomainId])
REFERENCES [dbo].[CompanyAcDomains] ([CompanyAcServerId])
GO

ALTER TABLE [dbo].[CompanyEventQuizMapping] CHECK CONSTRAINT [FK_CompanyEventQuizMapping_CompanyAcDomains]
GO

ALTER TABLE [dbo].[CompanyEventQuizMapping]  WITH CHECK ADD  CONSTRAINT [FK_CompanyEventQuizMapping_Quiz] FOREIGN KEY([PreQuizId])
REFERENCES [dbo].[Quiz] ([quizId])
GO

ALTER TABLE [dbo].[CompanyEventQuizMapping] CHECK CONSTRAINT [FK_CompanyEventQuizMapping_Quiz]
GO

ALTER TABLE [dbo].[CompanyEventQuizMapping]  WITH CHECK ADD  CONSTRAINT [FK_CompanyEventQuizMapping_Quiz_Cascade] FOREIGN KEY([PostQuizId])
REFERENCES [dbo].[Quiz] ([quizId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CompanyEventQuizMapping] CHECK CONSTRAINT [FK_CompanyEventQuizMapping_Quiz_Cascade]
GO





alter table dbo.QuizResult
drop constraint FK_QuizResult_EventQuizMapping

alter table dbo.QuizResult
drop column EventQuizMappingId

alter table dbo.QuizResult
add EventQuizMappingId int
GO
alter table dbo.QuizResult
ADD CONSTRAINT FK_QuizResult_EventQuizMapping FOREIGN KEY (EventQuizMappingId)     
    REFERENCES dbo.CompanyEventQuizMapping (CompanyEventQuizMappingId)     
   