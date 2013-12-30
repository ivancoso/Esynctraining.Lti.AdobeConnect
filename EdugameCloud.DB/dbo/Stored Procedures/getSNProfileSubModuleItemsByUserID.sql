-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 05.23.2013
-- Usage:		Admin
-- Description:	returns all SMIs spicific for 
--				the SNProfile 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getSNProfileSubModuleItemsByUserID]
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
        SubModuleCategory AS SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
        SNProfile AS Q ON Q.subModuleItemId = SMI.subModuleItemId
        
WHERE   (SMI.createdBy = @userId) AND (SMC.userId = @userId)

END