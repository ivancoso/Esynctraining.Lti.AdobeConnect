alter table Question
add htmlText nvarchar(2000)

GO


-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	is used to get a list of question for 
--				current quiz by subModuleItemId.
-- =============================================
alter PROCEDURE [dbo].[getSMIQuestionsBySMIId]
	@subModuleItemId int = null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,
		 Q.htmlText,
		 Q.questionOrder,
		 Q.questionTypeId,
		 Q.instruction,
		 
		 Q.incorrectMessage,
		 Q.correctReference,
		 Q.correctMessage,
		 Q.hint,
		 Q.imageId,
		 Q.scoreValue,
		 Q.randomizeAnswers,
		 Q.[rows],
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
		    WHEN Q.questionTypeId = 12 THEN qr.pageNumber 
			WHEN Q.questionTypeId = 13 THEN ql.pageNumber 
			WHEN Q.questionTypeId = 14 THEN qw.pageNumber 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.pageNumber 
			WHEN Q.questionTypeId = 10 or Q.questionTypeId = 11 THEN qo.pageNumber 
			WHEN Q.questionTypeId = 2 THEN qtf.pageNumber 
			ELSE null END AS pageNumber,
		 CASE 
			WHEN Q.questionTypeId = 14 THEN qw.totalWeightBucket 
			ELSE null END AS totalWeightBucket,
		 CASE 
			WHEN Q.questionTypeId = 14 THEN qw.weightBucketType 
			ELSE null END AS weightBucketType,
		CASE 
			WHEN Q.questionTypeId = 12 THEN qr.isAlwaysRateDropdown
			ELSE null END AS isAlwaysRateDropdown,
		 I.x,
		 I.y,
		 I.height,
		 I.width
		   
FROM     Question Q 
	     left outer join QuestionForLikert ql on ql.questionId = q.questionId
		 left outer join QuestionForOpenAnswer qo on qo.questionId = q.questionId
		 left outer join QuestionForRate qr on qr.questionId = q.questionId
		 left outer join QuestionForTrueFalse qtf on qtf.questionId = q.questionId
		 left outer join QuestionForWeightBucket qw on qw.questionId = q.questionId
		 left outer join QuestionForSingleMultipleChoice qc on qc.questionId = q.questionId 
		 INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId LEFT OUTER JOIN
         [File] I ON I.fileId = Q.imageId
         
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END
