-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	is used to get a list of distractors 
--				for each question of current quiz 
--				by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getSMIDistractorsBySMIId]
	@subModuleItemId int = null
AS
BEGIN

SELECT   Distinct
		 D.distractorId,
		 D.questionId,
		 D.distractorType,
		 D.distractor,
		 D.distractorOrder,
		 D.isCorrect,
		 D.imageId,
		 I.x,
		 I.y,
		 I.height,
		 I.width
		   
FROM     Distractor D INNER JOIN
         Question Q ON D.questionID = Q.questionId INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId LEFT OUTER JOIN
         [File] I ON I.fileId = D.imageID
         
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1 AND D.isActive = 1

END