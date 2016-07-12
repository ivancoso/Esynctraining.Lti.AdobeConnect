INSERT [dbo].[LmsProvider] ([lmsProviderId], [lmsProvider], [shortName], [configurationUrl], [userGuideFileUrl]) VALUES(4, 'Blackboard', 'blackboard', NULL, NULL)

SET IDENTITY_INSERT [dbo].[LmsQuestionType] ON

GO

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(34, 4, 2, 'True/False', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(35, 4, 1, 'Multiple Choice', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(36, 4, 1, 'Multiple Answer', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(37, 4, 2, 'Either/Or', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(38, 4, 16, 'Essay', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(39, 4, 15, 'Short Response', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(40, 4, 4, 'Fill in the Blank', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(41, 4, 4, 'Fill in the Blank Plus', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(42, 4, 20, 'Jumbled Sentence', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(43, 4, 17, 'Numeric', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(44, 4, 15, 'Quiz Bowl', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(45, 4, 1, 'Opinion Scale', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(46, 4, 8, 'Ordering', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(47, 4, 3, 'Matching', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(48, 4, 18, 'Calculated', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(49, 4, 15, 'File Upload', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(50, 4, 6, 'Hot Spot', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(51, 4, 13, 'Opinion Scale', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(52, 4, 10, 'Short Response', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(53, 4, 4, 'Fill in the Blank', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(54, 4, 10, 'Quiz Bowl', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(55, 4, 11, 'Essay', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(56, 4, 10, 'Numeric', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(57, 4, 10, 'Calculated', 3)

SET IDENTITY_INSERT [dbo].[LmsQuestionType] OFF

GO

