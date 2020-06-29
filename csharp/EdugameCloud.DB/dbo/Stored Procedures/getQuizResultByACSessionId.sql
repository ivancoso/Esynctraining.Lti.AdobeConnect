CREATE PROCEDURE [dbo].[getQuizResultByACSessionId]  
	@acSessionId int = null,@subModuleItemID int = null
AS
BEGIN
select sub.quizResultId, sub.participantName, sub.acEmail, sub.score, sub.TotalQuestion, sub.startTime, sub.endTime, 
		 ROW_NUMBER() OVER (ORDER BY sub.score desc, sub.dateDifference asc) AS position, sub.isCompleted, sub.appMaximizedTime, sub.appInFocusTime, sub.passingScore, sub.isPostQuiz, sub.quizResultGuid from (
SELECT   QR.quizResultId,
		 QR.participantName,	
		 QR.acEmail,	 
		 QR.score,
		 (select Count(Q.questionid) from Question Q where Q.subModuleItemId=@subModuleItemID) as TotalQuestion,
	 	 QR.startTime,
		 QR.endTime,
		 DATEDIFF(second, QR.startTime, QR.endTime) as dateDifference,
		 QR.isCompleted,
		 QR.appMaximizedTime as appMaximizedTime,
		 QR.appInFocusTime as appInFocusTime,
		 Q.passingScore as passingScore,
		 q.isPostQuiz,
		 QR.[guid] as quizResultGuid
		 
		    
FROM     Quiz Q INNER JOIN
         QuizResult QR ON Q.quizId = QR.quizId


WHERE    QR.acSessionId = @acSessionId and Q.submoduleItemId = @subModuleItemID
) as sub


END