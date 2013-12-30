-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Admin
-- Description:	returns all SMIs spicific for 
--				the Quiz and for the 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getQuizSubModuleItemsByUserID]
	@userId int = null
AS
BEGIN

SELECT  SMI.subModuleItemId, 
		SMC.subModuleId,
		SMI.subModuleCategoryId,
		SMI.createdBy,
		SMI.isShared,
		SMI.modifiedBy,
		SMI.dateCreated,
		SMI.dateModified,
		SMI.isActive
		
FROM    SubModuleItem AS SMI INNER JOIN
        SubModuleCategory AS SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT OUTER JOIN
        Quiz AS Q ON Q.subModuleItemId = SMI.subModuleItemId
        
WHERE   (SMI.createdBy = @userId) AND (SMC.userId = @userId) AND (Q.quizId <> '')

END