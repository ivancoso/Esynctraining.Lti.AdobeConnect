	
DELETE FROM [dbo].[LmsProvider]
WHERE [lmsProviderId] = 9
GO

INSERT [dbo].[LmsProvider] ([lmsProviderId], [lmsProvider], [shortName], [configurationUrl], [userGuideFileUrl]) 
VALUES(9, 'Schoology', 'schoology', NULL, NULL)
GO

