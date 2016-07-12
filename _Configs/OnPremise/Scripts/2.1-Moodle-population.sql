INSERT [dbo].[LmsProvider] ([lmsProviderId], [lmsProvider], [shortName], [configurationUrl], [userGuideFileUrl]) VALUES(1, 'Moodle', 'moodle', NULL, NULL)

SET IDENTITY_INSERT [dbo].[LmsQuestionType] ON

GO

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(17, 1, 1, 'multichoice', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(18, 1, 2, 'truefalse', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(19, 1, 3, 'match', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(20, 1, 4, 'multianswer', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(21, 1, 10, 'textfield', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(22, 1, 11, 'textarea', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(23, 1, 15, 'shortanswer', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(24, 1, 16, 'essay', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(25, 1, 17, 'numerical', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(26, 1, 18, 'calculated', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(27, 1, 19, 'calculatedmulti', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(28, 1, 21, 'description', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(29, 1, 18, 'calculatedsimple', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(30, 1, 1, 'multichoicerated', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(31, 1, 17, 'numeric', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(32, 1, 21, 'label', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(33, 1, 21, 'info', NULL)

SET IDENTITY_INSERT [dbo].[LmsQuestionType] OFF

GO