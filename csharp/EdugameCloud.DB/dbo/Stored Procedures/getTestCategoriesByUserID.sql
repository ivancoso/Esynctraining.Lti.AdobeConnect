
-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Admin
-- Description:	returns all categories spicific for 
--				the Test and for the current user only 
--				(not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getTestCategoriesByUserID]
	@userId int = null
AS
BEGIN

SELECT  DISTINCT SMC.subModuleCategoryId,
		SMC.userId, 
		SMC.subModuleId, 
		SMC.categoryName, 
		SMC.modifiedBy,
		SMC.dateModified,
		SMC.isActive
		
FROM    SubModuleItem SMI INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT JOIN
        Test T ON T.subModuleItemId = SMI.subModuleItemId
          
WHERE   SMC.userId = @userId AND SMI.createdBy = @userId AND T.testId <> ''

END