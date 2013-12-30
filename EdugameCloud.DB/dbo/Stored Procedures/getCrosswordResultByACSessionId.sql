-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Admin
-- Description:	is used to get a list of crosswords results 
--				by acSessionId
-- =============================================
CREATE PROCEDURE [dbo].[getCrosswordResultByACSessionId]  
	@acSessionId int = null
AS
BEGIN

SELECT   AR.appletResultId,
		 AR.participantName,
		 AI.documentXML,
		 AR.score,
	 	 AR.startTime,
		 AR.endTime,
		 ROW_NUMBER() OVER (ORDER BY AR.score DESC) AS position
		    
FROM     AppletItem AI INNER JOIN
         AppletResult AR ON AI.appletItemId = AR.appletItemId

WHERE    AR.acSessionId = @acSessionId

END