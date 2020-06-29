alter table dbo.Distractor
add leftImageId UNIQUEIDENTIFIER NULL

alter table dbo.Distractor
add rightImageId UNIQUEIDENTIFIER NULL

alter table dbo.Distractor
add CONSTRAINT [FK_Distractor_LeftImage] FOREIGN KEY (leftImageId) REFERENCES [dbo].[File] ([fileId])

alter table dbo.Distractor
add CONSTRAINT [FK_Distractor_RightImage] FOREIGN KEY (rightImageId) REFERENCES [dbo].[File] ([fileId])


GO
-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	is used to get a list of distractors 
--				for each question of current quiz 
--				by subModuleItemId.
-- =============================================
alter PROCEDURE [dbo].[getSMIDistractorsBySMIId]
	@subModuleItemId int = null
AS
BEGIN

SELECT   Distinct
		 D.distractorID,
		 D.questionId,
		 D.distractorType,
		 D.distractor,
		 D.distractorOrder,
		 D.isCorrect,
		 D.imageId,
		 D.leftImageId,
		 D.rightImageId,
		 I.x,
		 I.y,
		 I.height,
		 I.width
		   
FROM     Distractor D INNER JOIN
         Question Q ON D.questionID = Q.questionId INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId LEFT OUTER JOIN
         [File] I ON I.fileId = D.imageId      LEFT OUTER JOIN  [File] LeftI ON LeftI.fileId = D.leftImageId
		 LEFT OUTER JOIN  [File] RightI ON RightI.fileId = D.rightImageId
         
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1 AND D.isActive = 1

END