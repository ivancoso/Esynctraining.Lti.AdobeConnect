
-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current quiz by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getQuizQuestionsForAdminBySMIId]
	@subModuleItemId int = null, @acSessionID int =null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,	
		 Q.questionTypeId,
		 QT.type as questionTypeName,
		 (SELECT  
		 SUM(case QT.isCorrect
		  when 1
		  then 1
		  end)	    
FROM     QuizResult QR 
         LEFT join  QuizQuestionResult QT on QT.quizResultId = qr.quizResultId
         LefT join Question que on que.questionId = qt.questionId

WHERE    QR.acSessionId = @acSessionID and que.questionId = Q.questionId group by que.questionId) as correctAnswerCount
		   
FROM     Question Q INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId INNER JOIN
         QuestionType QT ON Q.questionTypeId = QT.questionTypeId
     
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END