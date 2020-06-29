

-- =============================================
-- Author:		Anton Abyzov
-- Create date: 10.03.2016
-- Usage:		Public
-- Description:	adjusted to get live and post quizzes 
-- =============================================
CREATE PROCEDURE [dbo].[getQuizSessionsByUserId]  
	@userId int = null
AS
BEGIN

select 
  lang.[language], ac.acSessionId, qr.EventQuizMappingId as eventQuizMappingId, 
  (select Count(Q.questionid) from Question Q where Q.subModuleItemId=ac.subModuleItemId and q.isActive = 1 ) as TotalQuestion,
  q.submoduleItemId as subModuleItemId, ac.dateCreated, ac.includeACEmails as includeAcEmails, cat.categoryName, um.acUserModeId, 
   COUNT(QR.quizResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.quizResultId)
       FROM QuizResult QR JOIN Quiz on Quiz.QuizId = QR.QuizId 
       WHERE QR.score > 0 AND ac.acSessionId = QR.acSessionId and Quiz.isPostQuiz = q.isPostQuiz) AS activeParticipants,
       (SELECT SUM(score)from QuizResult JOIN Quiz on Quiz.QuizId = QuizResult.QuizId where acSessionId = ac.acSessionId and Quiz.isPostQuiz = q.isPostQuiz ) AS TotalScore,          
	   --q.isPostQuiz,
	   q.quizName,
    ac.userId
from ACSession ac, QuizResult qr, Quiz q, SubmoduleItem smi, SubmoduleCategory cat, [Language] lang, [ACUserMode] um      --, [User] u

where qr.acSessionId= ac.acSessionId and qr.quizId = q.quizId and ac.userId = @userId -- and ac.userId = u.userId 
and cat.submoduleCategoryId = smi.submoduleCategoryId and ac.languageId = lang.languageId and um.acuserModeId = ac.acuserModeId
and smi.submoduleItemId = q.submoduleItemId
and ( (smi.submoduleItemId = ac.submoduleItemId) or q.isPostQuiz = 1)
group by lang.[language], QR.EventQuizMappingId, q.submoduleItemId, ac.submoduleItemId, ac.dateCreated, ac.includeACEmails, q.quizName, q.quizId, cat.categoryName, um.acUserModeId, ac.userId, ac.acSessionId, q.isPostQuiz
END