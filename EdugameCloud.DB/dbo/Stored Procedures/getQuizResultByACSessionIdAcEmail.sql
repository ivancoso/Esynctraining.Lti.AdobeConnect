
CREATE PROCEDURE [dbo].[getQuizResultByACSessionIdAcEmail]
(
	@acSessionId		INT,
	@subModuleItemID	INT,
	@acEmail			NVARCHAR(500)
) 
AS
BEGIN

SELECT
	sub.quizResultId, 
	sub.participantName,
	sub.acEmail,
	sub.score,
	sub.TotalQuestion, -- TRICK: TotalQuestion
	sub.startTime,
	sub.endTime, 
	ROW_NUMBER() OVER (ORDER BY sub.score desc, sub.dateDifference asc) AS position,
	sub.isCompleted,
	sub.appInFocusTime,
	sub.appMaximizedTime,
	sub.passingScore
FROM
(
	SELECT  QR.quizResultId,
			QR.participantName,	
			QR.acEmail,	 
			QR.score,
			(SELECT Count(Q.questionid) FROM Question Q WHERE Q.subModuleItemId = @subModuleItemID) AS TotalQuestion, -- TRICK: TotalQuestion
			QR.startTime,
			QR.endTime,
			DATEDIFF(second, QR.startTime, QR.endTime) AS dateDifference,
			QR.isCompleted,
		    QR.appMaximizedTime as appMaximizedTime,
			QR.appInFocusTime as appInFocusTime,
			Qz.passingScore as passingScore
	FROM Quiz Qz
		INNER JOIN         QuizResult QR ON Qz.quizId = QR.quizId
	WHERE QR.acSessionId = @acSessionId AND QR.acEmail = @acEmail
) AS sub


END
GO
