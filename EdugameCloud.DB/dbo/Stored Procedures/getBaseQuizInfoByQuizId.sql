-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Public
-- Description:	returns base information about 
--				quiz by its ID
-- =============================================
CREATE PROCEDURE [dbo].[getBaseQuizInfoByQuizId]
	@quizID int = null
AS
BEGIN
SELECT  Qz.description,
        Qz.subModuleItemId, 
		QF.quizFormatId, 
		ST.scoreTypeId
		 
FROM    SubModuleItem AS SMI INNER JOIN
        Quiz AS Qz ON SMI.subModuleItemId = Qz.subModuleItemId INNER JOIN
        QuizFormat AS QF ON Qz.quizFormatId = QF.quizFormatId INNER JOIN
        ScoreType AS ST ON Qz.scoreTypeId = ST.scoreTypeId 
        
WHERE   Qz.quizId = @quizID

END