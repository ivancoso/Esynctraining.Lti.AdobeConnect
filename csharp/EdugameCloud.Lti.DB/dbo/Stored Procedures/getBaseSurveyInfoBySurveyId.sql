
-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	returns base information about 
--				quiz by its ID
-- =============================================
CREATE PROCEDURE [dbo].[getBaseSurveyInfoBySurveyId]
	@surveyId int = null
AS
BEGIN
SELECT  Sr.[description],
        Sr.subModuleItemId, 
		Sr.surveyGroupingTypeId
		 
FROM    SubModuleItem AS SMI INNER JOIN
        Survey AS Sr ON SMI.subModuleItemId = Sr.subModuleItemId INNER JOIN
        SurveyGroupingType AS SGT ON Sr.surveyGroupingTypeId = SGT.surveyGroupingTypeId 
        
WHERE   Sr.surveyId = @surveyId

END