


-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 05.15.2013
-- Usage:		Admin
-- Description:	is used to get a list of sn sessions  by userId for Admin Reporting
-- =============================================
CREATE PROCEDURE [dbo].[getSNSessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 	   
	   ACS.acSessionId, 	
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   COUNT(SNM.snMemberId) AS totalParticipants, 
	   (SELECT COUNT(SNM.snMemberId) FROM SNMember SNM WHERE ACS.acSessionId = SNM.acSessionId and SNM.isBlocked = 0) AS activeParticipants,
	   SNP.snProfileId, 
       SNP.profileName, 
	   SNG.snGroupDiscussionId,
	   SNG.groupDiscussionTitle,
       u.userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId LEFT OUTER JOIN
	   SNGroupDiscussion SNG ON SNG.acSessionId = ACS.acSessionId INNER JOIN
       SNMember SNM ON ACS.acSessionId = SNM.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId LEFT OUTER JOIN
	   SNProfile SNP ON SNP.subModuleItemId = ACS.subModuleItemId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] u ON ACS.userId = u.userId
       
GROUP BY LNG.[language],  ACS.acSessionId, ACS.subModuleItemId, ACS.dateCreated, SNP.snProfileId, SNP.profileName, SMC.categoryName, ACUM.acUserModeId, 
SNG.snGroupDiscussionId, SNG.groupDiscussionTitle, u.userId

HAVING      (u.userId = @userId)

END