CREATE PROCEDURE getSurveyResultByACSessionId
(
	@acSessionId		INT,
	@subModuleItemId	INT
)
AS
BEGIN

SELECT   SR.surveyResultId,
		 SR.participantName,		 
		 SR.score,
		 (select Count(Q.questionid) from Question Q where Q.subModuleItemId=@subModuleItemID) as TotalQuestion,
	 	 SR.startTime,
		 SR.endTime,
		 SR.acEmail,
		 ROW_NUMBER() OVER (ORDER BY SR.score DESC) AS position
		    
FROM     Survey S INNER JOIN
         SurveyResult SR ON S.surveyId = SR.surveyId

WHERE    SR.acSessionId = @acSessionId

END
