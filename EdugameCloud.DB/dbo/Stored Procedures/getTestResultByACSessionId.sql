

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Admin
-- Description:	is used to get a list of test results 
--				by acSessionId
-- =============================================
CREATE PROCEDURE [dbo].[getTestResultByACSessionId]  
	@acSessionId int = null,@subModuleItemID int = null
AS
BEGIN

SELECT   TR.testResultId,
		 TR.participantName,		 
		 TR.score,
		 (select Count(Q.questionid) from Question Q where Q.subModuleItemId=@subModuleItemID) as TotalQuestion,
	 	 TR.startTime,
		 TR.endTime,
		 ROW_NUMBER() OVER (ORDER BY TR.score DESC) AS position
		    
FROM     Test T INNER JOIN
         TestResult TR ON T.testId = TR.testId

WHERE    TR.acSessionId = @acSessionId

END