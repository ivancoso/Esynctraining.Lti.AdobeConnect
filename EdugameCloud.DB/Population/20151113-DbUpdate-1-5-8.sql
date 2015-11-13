ALTER TABLE SurveyResult
	ADD [acEmail] NVARCHAR(500) NULL
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'getCrosswordSessionsByUserIdMeetingUrl') AND type IN ( N'P', N'PC' ) ) 
	DROP PROCEDURE getCrosswordSessionsByUserIdMeetingUrl
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'getQuizSessionsByUserIdMeetingUrl') AND type IN ( N'P', N'PC' ) ) 
	DROP PROCEDURE getQuizSessionsByUserIdMeetingUrl
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'getSurveySessionsByUserIdMeetingUrl') AND type IN ( N'P', N'PC' ) ) 
	DROP PROCEDURE getSurveySessionsByUserIdMeetingUrl
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'getTestSessionsByUserIdMeetingUrl') AND type IN ( N'P', N'PC' ) ) 
	DROP PROCEDURE getTestSessionsByUserIdMeetingUrl
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'getQuizResultByACSessionIdAcEmail') AND type IN ( N'P', N'PC' ) ) 
	DROP PROCEDURE getQuizResultByACSessionIdAcEmail
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'getSurveyResultByACSessionIdAcEmail') AND type IN ( N'P', N'PC' ) ) 
	DROP PROCEDURE getSurveyResultByACSessionIdAcEmail
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'getTestResultByACSessionIdAcEmail') AND type IN ( N'P', N'PC' ) ) 
	DROP PROCEDURE getTestResultByACSessionIdAcEmail
GO

-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getCrosswordSessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 
	   AR.appletItemId, 
	   AR.acSessionId, 
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.appletName,
	   COUNT(AR.appletResultId) AS totalParticipants, 
	   (SELECT COUNT(AR.appletResultId)
		FROM AppletResult AR
		WHERE AR.score > 0 AND ACS.acSessionId = AR.acSessionId) AS activeParticipants,
       AI.appletName, 
       usr.userId

FROM ACSession ACS 
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId 
	INNER JOIN       AppletResult AR ON ACS.acSessionId = AR.acSessionId 
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId 
	INNER JOIN         AppletItem AI ON AR.appletItemId = AI.appletItemId 
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI.subModuleItemId 
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId 
	INNER JOIN            [User] usr ON ACS.userId = usr.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language], AR.appletItemId, AR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.appletName, SMC.categoryName, ACS.acUserModeId, usr.userId, ACS.acSessionId

END
GO

-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getQuizSessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 	   
	   QR.acSessionId, 	
	   (select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.quizName,
	   COUNT(QR.quizResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.quizResultId)
       FROM QuizResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from QuizResult where acSessionId = QR.acSessionId ) AS TotalScore,
       AI.quizName, 
       USR.userId

FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId 
	INNER JOIN         QuizResult QR ON ACS.acSessionId = QR.acSessionId 
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId 
	INNER JOIN               Quiz AI ON QR.quizId = AI.quizId 
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId 
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  QR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.quizName, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END
GO

-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getSurveySessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 	   
	   SR.acSessionId, 	
	   (select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.surveyName,	 
	   COUNT(SR.surveyResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.surveyResultId)
       FROM SurveyResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from SurveyResult where acSessionId = SR.acSessionId ) AS TotalScore,
       USR.userId
       
FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId
	INNER JOIN       SurveyResult SR ON ACS.acSessionId = SR.acSessionId
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId
	INNER JOIN             Survey AI ON SR.surveyId = AI.surveyId
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  SR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.surveyName, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END
GO

-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getTestSessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 	   
	   TR.acSessionId, 	
	   --(select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.testName,	 
	   AI.passingScore,
	   COUNT(TR.testResultId) AS totalParticipants, 
	   (SELECT COUNT(TR.testResultId)
       FROM TestResult TR
       WHERE TR.score > 0 AND ACS.acSessionId = TR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from TestResult where acSessionId = TR.acSessionId) AS TotalScore,
	   (SELECT AVG(score) from TestResult where acSessionId = TR.acSessionId) AS avgScore,
       USR.userId
       
FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId
	INNER JOIN         TestResult TR ON ACS.acSessionId = TR.acSessionId
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId
	INNER JOIN               Test AI ON TR.testId = AI.testId
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  TR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.testName, AI.passingScore, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END
GO

CREATE PROCEDURE getQuizResultByACSessionIdAcEmail
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
	sub.isCompleted 
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
			QR.isCompleted
	FROM Quiz Qz
		INNER JOIN         QuizResult QR ON Qz.quizId = QR.quizId
	WHERE QR.acSessionId = @acSessionId AND QR.acEmail = @acEmail
) AS sub


END
GO

CREATE PROCEDURE getSurveyResultByACSessionIdAcEmail 
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
GO

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
GO
