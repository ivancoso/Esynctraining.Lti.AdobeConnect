-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.22.2013
-- Usage:		Public
-- Description:	is used to get a list of question for 
--				current quiz by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getQuizQuestionsBySMIId]
	@subModuleItemId int = null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,
		 Q.questionOrder,
		 Q.questionTypeId,
		 Q.instruction,
		 Q.incorrectMessage,
		 Q.correctMessage,
		 Q.hint,
		 Q.imageId,
		 Q.scoreValue,
		 I.x,
		 I.y,
		 I.height,
		 I.width
		   
FROM     Question Q INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId LEFT OUTER JOIN
         [Image] I ON I.imageId = Q.imageId
         
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END