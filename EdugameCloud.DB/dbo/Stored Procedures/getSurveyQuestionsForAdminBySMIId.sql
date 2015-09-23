

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current survey by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getSurveyQuestionsForAdminBySMIId]
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