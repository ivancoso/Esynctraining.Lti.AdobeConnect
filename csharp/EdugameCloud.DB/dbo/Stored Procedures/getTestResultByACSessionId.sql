

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.10.2014
-- Usage:		Admin
-- Description:	is used to get a list of test results 
--				by acSessionId
-- =============================================
CREATE PROCEDURE [dbo].[getTestResultByACSessionId]  
	@acSessionId int = null,@subModuleItemId int = null
AS
BEGIN

SELECT   TR.testResultId,
		 TR.participantName,		 
		 TR.acEmail,
		 TR.score,
		 (select Count(Q.questionId) from Question Q where Q.subModuleItemId=@subModuleItemId) as TotalQuestion,
	 	 TR.startTime,
		 TR.endTime,
		 ROW_NUMBER() OVER (ORDER BY TR.score DESC) AS position,
		 TR.isCompleted
		    
FROM     Test T INNER JOIN
         TestResult TR ON T.testId = TR.testId

WHERE    TR.acSessionId = @acSessionId

END