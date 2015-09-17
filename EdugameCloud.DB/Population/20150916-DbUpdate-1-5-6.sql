USE [EduGameCloud.Dev]
GO

/****** Object:  StoredProcedure [dbo].[getTestSessionsByUserId]    Script Date: 16.Sep.15 5:33:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.10.2014
-- Usage:		Admin
-- Description:	is used to get a list of test sessions 
--              by userId for Admin Reporting
-- =============================================
ALTER PROCEDURE [dbo].[getTestSessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 	   
	   TR.acSessionId, 	
	   --(select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.testName,	 
	   AI.passingScore,
	   COUNT(TR.testResultId) AS totalParticipants, 
	   (SELECT COUNT(TR.testResultId)
       FROM TestResult TR
       WHERE TR.score > 0 AND ACS.acSessionId = TR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from TestResult where acSessionId = TR.acSessionId) AS TotalScore,
	   (SELECT AVG(score) from TestResult where acSessionId = TR.acSessionId) AS avgScore,
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId INNER JOIN
       TestResult TR ON ACS.acSessionId = TR.acSessionId INNER JOIN
	   
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       Test AI ON TR.testId = AI.testId INNER JOIN
	   
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language],  TR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.testName, AI.passingScore, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId)

END

GO

-- LmsUser AC Password schema update
set xact_abort on;
go

begin transaction;
go

alter table LmsUser add
  sharedKey nvarchar(max),
  acPasswordData nvarchar(max);
go

commit;
go
--

