-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getQuizSessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 	   
	   QR.acSessionId, 	
	   (select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.quizName,
	   COUNT(QR.quizResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.quizResultId)
       FROM QuizResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from QuizResult where acSessionId = QR.acSessionId ) AS TotalScore,
       AI.quizName, 
       USR.userId

FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId 
	INNER JOIN         QuizResult QR ON ACS.acSessionId = QR.acSessionId 
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId 
	INNER JOIN               Quiz AI ON QR.quizId = AI.quizId 
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId 
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  QR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.quizName, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END