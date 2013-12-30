

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of quiz results 
--				by acSessionId
-- =============================================
CREATE PROCEDURE [dbo].[getSurveyResultByACSessionId] 
	@acSessionId int = null,@subModuleItemID int = null
AS
BEGIN

SELECT   SR.surveyResultId,
		 SR.participantName,		 
		 SR.score,
		 (select Count(Q.questionid) from Question Q where Q.subModuleItemId=@subModuleItemID) as TotalQuestion,
	 	 SR.startTime,
		 SR.endTime,
		 ROW_NUMBER() OVER (ORDER BY SR.score DESC) AS position
		    
FROM     Survey S INNER JOIN
         SurveyResult SR ON S.surveyId = SR.surveyId

WHERE    SR.acSessionId = @acSessionId

END