-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Public
-- Description:	is used to get a list of shared crosswords (not own) 
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getSharedForUserCrosswordsByUserId] 
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
        SubModuleItem SMI ON AI.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' AND SMI.isShared = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
		INNER JOIN [User] U2 ON (SMI.createdBy = U2.userId AND U2.[status] = 1)
        
WHERE   U2.userId != @userId AND U2.companyId IN (SELECT TOP 1 companyId FROM [User] WHERE userId = @userId)

END