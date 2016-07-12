INSERT [dbo].[LmsProvider] ([lmsProviderId], [lmsProvider], [shortName], [configurationUrl], [userGuideFileUrl]) VALUES(2, 'Canvas', 'canvas', NULL, NULL)

SET IDENTITY_INSERT [dbo].[LmsQuestionType] ON

GO

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(1, 2, 1, 'multiple_choice_question', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(2, 2, 1, 'multiple_answers_question', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(3, 2, 2, 'true_false_question', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(5, 2, 16, 'essay_question', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(6, 2, 17, 'numerical_question', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(8, 2, 15, 'short_answer_question', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(9, 2, 20, 'multiple_dropdowns_question', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(10, 2, 4, 'fill_in_multiple_blanks_question', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(11, 2, 3, 'matching_question', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(12, 2, 18, 'calculated_question', 2)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(16, 2, 21, 'text_only_question', NULL)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(58, 2, 10, 'short_answer_question', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(59, 2, 11, 'essay_question', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(60, 2, 10, 'numerical_question', 3)

INSERT [dbo].[LmsQuestionType] ([lmsQuestionTypeId], [lmsProviderId], [questionTypeId], [lmsQuestionType], [subModuleId]) VALUES(61, 2, 10, 'calculated_question', 3)

SET IDENTITY_INSERT [dbo].[LmsQuestionType] OFF

GO

