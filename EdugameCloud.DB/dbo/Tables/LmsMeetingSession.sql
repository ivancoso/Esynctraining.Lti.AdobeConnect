CREATE TABLE [dbo].[LmsMeetingSession](
	[lmsMeetingSessionId] [int] IDENTITY(1,1) NOT NULL,
	[eventId] [nvarchar](50) NULL,
	[name] [nvarchar](200) NOT NULL,
	[startDate] [datetime2](7) NOT NULL,
	[endDate] [datetime2](7) NOT NULL,
	[lmsCourseMeetingId] [int] NOT NULL,
	[summary] [nvarchar](200) NULL,
CONSTRAINT [PK_LmsMeetingSession] PRIMARY KEY CLUSTERED ([lmsMeetingSessionId] ASC),
CONSTRAINT [FK_LmsMeetingSession_LmsCourseMeeting] FOREIGN KEY([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
);