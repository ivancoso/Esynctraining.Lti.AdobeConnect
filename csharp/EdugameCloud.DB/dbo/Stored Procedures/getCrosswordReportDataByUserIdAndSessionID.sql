
-- =============================================
-- Author:		Anju 
-- Create date: 02.18.2013
-- Usage:		Admin
-- Description:	is used to get a list of crosswords sessions 
--              by userId //Please delete this once the stupid bug 
--				to get reporting data from Flex to cf.
-- =============================================
CREATE PROCEDURE [dbo].[getCrosswordReportDataByUserIdAndSessionID]  
	@userId int = null , @sessionid int=null
AS
BEGIN

SELECT LNG.[language], 
	   AR.appletItemId, 
	   AR.acSessionId, 
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.appletName,
	   COUNT(AR.appletResultId) AS Total, 
	   (SELECT COUNT(AR.appletResultId)
       FROM AppletResult AR
       WHERE AR.score > 0 AND ACS.acSessionId = AR.acSessionId) AS Active,
       AI.appletName, 
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG ON ACS.languageId = LNG.languageId INNER JOIN
       AppletResult AR ON ACS.acSessionId = AR.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       AppletItem AI ON AR.appletItemId = AI.appletItemId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language], AR.appletItemId, AR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.appletName, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId) AND (ACS.acSessionId = @sessionid)

END