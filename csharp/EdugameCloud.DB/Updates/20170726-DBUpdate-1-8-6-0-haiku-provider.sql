	
DELETE FROM [dbo].[LmsProvider]
WHERE [lmsProviderId] = 10
GO

INSERT [dbo].[LmsProvider] ([lmsProviderId], [lmsProvider], [shortName], [configurationUrl], [userGuideFileUrl]) 
VALUES(10, 'Haiku', 'haiku', NULL, NULL)
GO

