-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getTestSessionsByUserIdMeetingUrl
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
	   TR.acSessionId, 	
	   --(select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.testName,	 
	   AI.passingScore,
	   COUNT(TR.testResultId) AS totalParticipants, 
	   (SELECT COUNT(TR.testResultId)
       FROM TestResult TR
       WHERE TR.score > 0 AND ACS.acSessionId = TR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from TestResult where acSessionId = TR.acSessionId) AS TotalScore,
	   (SELECT AVG(score) from TestResult where acSessionId = TR.acSessionId) AS avgScore,
       USR.userId
       
FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId
	INNER JOIN         TestResult TR ON ACS.acSessionId = TR.acSessionId
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId
	INNER JOIN               Test AI ON TR.testId = AI.testId
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  TR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.testName, AI.passingScore, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END