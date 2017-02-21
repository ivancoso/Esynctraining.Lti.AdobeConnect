/****** Object:  Table [dbo].[QuizQuestionResultAnswer]    Script Date: 1/23/2017 10:29:23 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuizQuestionResultAnswer](
	[quizQuestionResultAnswerId] [int] IDENTITY(1,1) NOT NULL,
	[quizQuestionResultId] [int] NOT NULL,
	[value] [nvarchar](500) NOT NULL,
	[quizDistractorAnswerId] [int] NULL,
 CONSTRAINT [PK_QuizQuestionResultAnswer] PRIMARY KEY CLUSTERED 
(
	[quizQuestionResultAnswerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuizQuestionResultAnswer]  WITH CHECK ADD  CONSTRAINT [FK_QuizQuestionResultAnswer_QuizQuestionResult] FOREIGN KEY([quizQuestionResultId])
REFERENCES [dbo].[QuizQuestionResult] ([quizQuestionResultId])
GO

ALTER TABLE [dbo].[QuizQuestionResultAnswer] CHECK CONSTRAINT [FK_QuizQuestionResultAnswer_QuizQuestionResult]
GO

ALTER TABLE [dbo].[QuizQuestionResultAnswer]  WITH CHECK ADD  CONSTRAINT [FK_QuizQuestionResultAnswer_DistractorAnswer] FOREIGN KEY([quizDistractorAnswerId])
REFERENCES [dbo].[Distractor] ([distractorId])
GO

ALTER TABLE [dbo].[QuizQuestionResultAnswer] CHECK CONSTRAINT [FK_QuizQuestionResultAnswer_DistractorAnswer]
GO


