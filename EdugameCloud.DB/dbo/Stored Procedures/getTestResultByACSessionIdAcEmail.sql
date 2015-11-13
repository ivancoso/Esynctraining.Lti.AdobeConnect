
CREATE PROCEDURE getTestResultByACSessionIdAcEmail
(
	@acSessionId		INT,
	@subModuleItemID	INT,
	@acEmail			NVARCHAR(500)
)
AS
BEGIN

SELECT   TR.testResultId,
		 TR.participantName,		 
		 TR.acEmail,
		 TR.score,
		 (SELECT COUNT(Q.questionid) FROM Question Q WHERE Q.subModuleItemId = @subModuleItemID) AS TotalQuestion,
	 	 TR.startTime,
		 TR.endTime,
		 ROW_NUMBER() OVER (ORDER BY TR.score DESC) AS position,
		 TR.isCompleted
		    
FROM Test T
	INNER JOIN TestResult TR ON T.testId = TR.testId
WHERE TR.acSessionId = @acSessionId AND TR.acEmail = @acEmail

END