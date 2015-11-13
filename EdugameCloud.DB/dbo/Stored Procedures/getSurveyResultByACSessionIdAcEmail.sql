﻿CREATE PROCEDURE [dbo].[getSurveyResultByACSessionIdAcEmail] 
(
	@acSessionId		INT,
	@subModuleItemID	INT,
	@acEmail			NVARCHAR(500)
)
AS
BEGIN

SELECT   SR.surveyResultId,
		 SR.participantName,		 
		 SR.score,
		 (SELECT COUNT(Q.questionid) FROM Question Q WHERE Q.subModuleItemId = @subModuleItemID) AS TotalQuestion,
	 	 SR.startTime,
		 SR.endTime,
		 ROW_NUMBER() OVER (ORDER BY SR.score DESC) AS position
		    
FROM     Survey S 
	INNER JOIN SurveyResult SR ON S.surveyId = SR.surveyId
WHERE    SR.acSessionId = @acSessionId AND SR.acEmail = @acEmail

END