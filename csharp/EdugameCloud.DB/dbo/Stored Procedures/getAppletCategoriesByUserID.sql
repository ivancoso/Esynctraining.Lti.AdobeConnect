-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Admin
-- Description:	returns all categories spicific for 
--				the appletItem (crossword) and for the 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getAppletCategoriesByUserID]
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
        AppletItem AI ON AI.subModuleItemId = SMI.subModuleItemId 
          
WHERE   SMC.userId = @userId AND SMI.createdBy = @userId AND AI.appletItemId <> ''

END