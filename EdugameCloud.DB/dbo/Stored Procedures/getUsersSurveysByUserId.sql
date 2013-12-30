
-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	is used to get a list of shared quizzes
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getUsersSurveysByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		S.surveyId,
		S.surveyName,
		S.[description],
		S.surveyGroupingTypeId
		
FROM    Survey S INNER JOIN
        SubModuleItem SMI ON S.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
        LEFT OUTER JOIN [User] U2 ON SMI.createdBy = U2.userId
        
WHERE U2.userId = @userId
      
END