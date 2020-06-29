-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.22.2013
-- Usage:		Public
-- Description:	is used to get a list of distractors 
--				for each question of current quiz 
--				by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getQuizDistractorsBySMIId]
	@subModuleItemId int = null
AS
BEGIN

SELECT   D.questionId,
		 D.distractorId,
		 D.distractor,
		 D.distractorOrder,
		 D.isCorrect,
		 D.imageId,
		 I.imageId,
		 I.x,
		 I.y,
		 I.height,
		 I.width
		   
FROM     Distractor D INNER JOIN
         Question Q ON D.questionID = Q.questionId INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId LEFT OUTER JOIN
         Image I ON I.imageId = D.imageID
         
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1 AND D.isActive = 1

END