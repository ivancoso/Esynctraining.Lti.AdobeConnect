-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getSurveySessionsByUserIdMeetingUrl
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
	   SR.acSessionId, 	
	   (select Count(Q.questionId) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.surveyName,	 
	   COUNT(SR.surveyResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.surveyResultId)
       FROM SurveyResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from SurveyResult where acSessionId = SR.acSessionId ) AS TotalScore,
       USR.userId
       
FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId
	INNER JOIN       SurveyResult SR ON ACS.acSessionId = SR.acSessionId
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId
	INNER JOIN             Survey AI ON SR.surveyId = AI.surveyId
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  SR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.surveyName, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END