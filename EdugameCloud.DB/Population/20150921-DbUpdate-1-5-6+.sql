ALTER TABLE CompanyLicense
    ADD hasApi BIT NULL
GO

UPDATE CompanyLicense
SET hasApi = 0
GO

ALTER TABLE CompanyLicense
    ALTER COLUMN hasApi BIT NOT NULL
GO

						
ALTER TABLE QuizResult
ALTER COLUMN participantName NVARCHAR(200) NOT NULL
GO
UPDATE QuizResult
SET participantName = RTRIM(LTRIM(participantName))
GO

ALTER TABLE AppletResult
ALTER COLUMN participantName NVARCHAR(200) NOT NULL
GO
UPDATE AppletResult
SET participantName = RTRIM(LTRIM(participantName))
GO

ALTER TABLE TestResult
ALTER COLUMN participantName NVARCHAR(200) NOT NULL
GO
UPDATE TestResult
SET participantName = RTRIM(LTRIM(participantName))
GO


ALTER TABLE ACSession
ALTER COLUMN meetingURL NVARCHAR(500) NOT NULL
GO
UPDATE ACSession
SET meetingURL = RTRIM(LTRIM(meetingURL))
GO

ALTER TABLE TestQuestionResult
ALTER COLUMN question NVARCHAR(500) NOT NULL
GO
UPDATE TestQuestionResult
SET question = RTRIM(LTRIM(question))
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current quiz by subModuleItemId.
-- =============================================
ALTER PROCEDURE [dbo].[getQuizQuestionsForAdminBySMIId]
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

WHERE    QR.acSessionId = @acSessionID and que.questionId = Q.questionID group by que.questionId) as correctAnswerCount
		   
FROM     Question Q INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId INNER JOIN
         QuestionType QT ON Q.questionTypeId = QT.questionTypeId
     
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current survey by subModuleItemId.
-- =============================================
ALTER PROCEDURE [dbo].[getSurveyQuestionsForAdminBySMIId]
	@subModuleItemId int = null, @acSessionID int =null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,	
		 Q.questionTypeId,
		 Q.questionOrder,
		 CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.restrictions 
			WHEN Q.questionTypeId = 10 or Q.questionTypeId = 11 THEN qo.restrictions 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.restrictions 
			ELSE null END AS restrictions,
		CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.allowOther 
			WHEN Q.questionTypeId = 13 THEN ql.allowOther 
			WHEN Q.questionTypeId = 14 THEN qw.allowOther 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.allowOther 
			ELSE null END AS allowOther,
		CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.isMandatory 
			WHEN Q.questionTypeId = 13 THEN ql.isMandatory 
			WHEN Q.questionTypeId = 14 THEN qw.isMandatory 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.isMandatory 
			WHEN Q.questionTypeId = 10 or Q.questionTypeId = 11 THEN qo.isMandatory 
			WHEN Q.questionTypeId = 2 THEN qtf.isMandatory 
			ELSE null END AS isMandatory,
		 CASE 
			WHEN Q.questionTypeId = 14 THEN qw.totalWeightBucket 
			ELSE null END AS totalWeightBucket,
		 CASE 
			WHEN Q.questionTypeId = 14 THEN qw.weightBucketType 
			ELSE null END AS weightBucketType,
		 QT.[type] as questionTypeName,
		 (SELECT  
		 SUM(case QT.isCorrect
		  when 1
		  then 1
		  end)	    
FROM     SurveyResult SR 
         LEFT join  SurveyQuestionResult QT on QT.surveyResultId = SR.surveyResultId
         LefT join Question que on que.questionId = qt.questionId

WHERE    SR.acSessionId = @acSessionID and que.questionId = Q.questionID group by que.questionId) as correctAnswerCount
		   
FROM     Question Q 
		 left outer join QuestionForLikert ql on ql.questionId = q.questionId
		 left outer join QuestionForOpenAnswer qo on qo.questionId = q.questionId
		 left outer join QuestionForRate qr on qr.questionId = q.questionId
		 left outer join QuestionForWeightBucket qw on qw.questionId = q.questionId
		 left outer join QuestionForTrueFalse qtf on qtf.questionId = q.questionId
		 left outer join QuestionForSingleMultipleChoice qc on qc.questionId = q.questionId
		 INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId INNER JOIN
         QuestionType QT ON Q.questionTypeId = QT.questionTypeId
     
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END

GO




-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current test by subModuleItemId.
-- =============================================
ALTER PROCEDURE [dbo].[getTestQuestionsForAdminBySMIId]
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