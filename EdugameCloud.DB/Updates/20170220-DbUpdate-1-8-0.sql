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
   



   -- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.10.2014
-- Usage:		Admin
-- Description:	is used to get a list of quiz sessions 
--              by userId for Admin Reporting
-- =============================================
ALTER PROCEDURE [dbo].[getQuizSessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 	   
	   QR.acSessionId, 	
	   QR.EventQuizMappingId as eventQuizMappingId,
	   (select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.quizName,	 
	   COUNT(QR.quizResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.quizResultId)
       FROM QuizResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from QuizResult where acSessionId = QR.acSessionId ) AS TotalScore,
       AI.quizName, 
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId INNER JOIN
       QuizResult QR ON ACS.acSessionId = QR.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       Quiz AI ON QR.quizId = AI.quizId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language],  QR.acSessionId, QR.EventQuizMappingId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.quizName, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId)

END





-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.10.2014
-- Usage:		Admin
-- Description:	is used to get a list of quiz results 
--				by acSessionId
-- =============================================
ALTER PROCEDURE [dbo].[getQuizResultByACSessionId]  
	@acSessionId int = null,@subModuleItemID int = null
AS
BEGIN
select sub.quizResultId, sub.participantName, sub.acEmail, sub.score, sub.TotalQuestion, sub.startTime, sub.endTime, 
		 ROW_NUMBER() OVER (ORDER BY sub.score desc, sub.dateDifference asc) AS position, sub.isCompleted from (
SELECT   QR.quizResultId,
		 QR.participantName,	
		 QR.acEmail,	 
		 QR.score,
		 (select Count(Q.questionid) from Question Q where Q.subModuleItemId=@subModuleItemID) as TotalQuestion,
	 	 QR.startTime,
		 QR.endTime,
		 DATEDIFF(second, QR.startTime, QR.endTime) as dateDifference,
		 QR.isCompleted,
		 QR.appMaximizedTime as appMaximizedTime,
		 QR.appInFocusTime as appInFocusTime
		 
		    
FROM     Quiz Q INNER JOIN
         QuizResult QR ON Q.quizId = QR.quizId

WHERE    QR.acSessionId = @acSessionId
) as sub


END


