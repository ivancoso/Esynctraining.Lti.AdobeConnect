

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of quiz sessions 
--              by userId for Admin Reporting
-- =============================================
CREATE PROCEDURE [dbo].[getSurveySessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 	   
	   SR.acSessionId, 	
	   (select Count(Q.questionId) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.surveyName,	 
	   COUNT(SR.surveyResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.surveyResultId)
       FROM SurveyResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from SurveyResult where acSessionId = SR.acSessionId ) AS TotalScore,
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId INNER JOIN
       SurveyResult SR ON ACS.acSessionId = SR.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       Survey AI ON SR.surveyId = AI.surveyId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language],  SR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.surveyName, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId)

END