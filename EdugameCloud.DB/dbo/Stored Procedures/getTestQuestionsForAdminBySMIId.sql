

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current test by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getTestQuestionsForAdminBySMIId]
	@subModuleItemId int = null, @acSessionID int =null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,	
		 Q.questionTypeId,
		 QT.type as questionTypeName,
		 (SELECT  
		 SUM(CAST(tqr.isCorrect AS INT))	    
FROM     TestResult tr 
         LEFT join  TestQuestionResult tqr on tqr.testResultId = tr.testResultId
         LefT join Question que on que.questionId = tqr.questionId

WHERE    tr.acSessionId = @acSessionID and que.questionId = Q.questionID group by que.questionId) as correctAnswerCount
		   
FROM     Question Q INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId INNER JOIN
         QuestionType QT ON Q.questionTypeId = QT.questionTypeId
     
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END