SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LmsCourseSection](
	[lmsCourseSectionId] [int] IDENTITY(1,1) NOT NULL,
	[lmsCourseMeetingId] [int] NOT NULL,
	[lmsId] [nvarchar](50) NOT NULL,
	[name] [nvarchar](256) NOT NULL
 CONSTRAINT [PK_LmsCourseSection] PRIMARY KEY CLUSTERED 
(
	[lmsCourseSectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LmsCourseSection]  WITH CHECK ADD  CONSTRAINT [FK_LmsCourseSection_LmsCourseMeeting] FOREIGN KEY([lmsCourseMeetingId])
REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
GO

ALTER TABLE [dbo].[LmsCourseSection] CHECK CONSTRAINT [FK_LmsCourseSection_LmsCourseMeeting]
GO


