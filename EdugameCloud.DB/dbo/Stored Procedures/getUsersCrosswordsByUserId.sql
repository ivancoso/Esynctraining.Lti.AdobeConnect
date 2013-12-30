-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Public
-- Description:	is used to get a list of crosswords 
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getUsersCrosswordsByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		AI.appletItemId,
		AI.appletName
		
FROM    AppletItem AI INNER JOIN
        SubModuleItem SMI ON AI.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
        LEFT OUTER JOIN [User] U2 ON SMI.createdBy = U2.userId
        
WHERE U2.userId = @userId
      
END