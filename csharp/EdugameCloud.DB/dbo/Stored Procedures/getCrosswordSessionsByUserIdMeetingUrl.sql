-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getCrosswordSessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 
	   AR.appletItemId, 
	   AR.acSessionId, 
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.appletName,
	   COUNT(AR.appletResultId) AS totalParticipants, 
	   (SELECT COUNT(AR.appletResultId)
		FROM AppletResult AR
		WHERE AR.score > 0 AND ACS.acSessionId = AR.acSessionId) AS activeParticipants,
       AI.appletName, 
       usr.userId

FROM ACSession ACS 
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId 
	INNER JOIN       AppletResult AR ON ACS.acSessionId = AR.acSessionId 
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId 
	INNER JOIN         AppletItem AI ON AR.appletItemId = AI.appletItemId 
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI.subModuleItemId 
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId 
	INNER JOIN            [User] usr ON ACS.userId = usr.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language], AR.appletItemId, AR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.appletName, SMC.categoryName, ACS.acUserModeId, usr.userId, ACS.acSessionId

END