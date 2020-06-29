-- declare @companyId int
-- select @companyId = CompanyLmsId
-- from CompanyLms
--  where consumerKey = 'fbf4fed1-375f-42a1-9d2c-163d566f4dd1'
  --print @companyId

--  insert into LmsCompanySetting values (@companyId, 'UseSakaiEvents', 'True')

CREATE TABLE [dbo].[LmsCalendarEvent](
	[lmsCalendarEventId] [int] IDENTITY(1,1) NOT NULL,
	[eventId] [nvarchar](50) NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[startDate] [datetime2](7) NOT NULL,
	[endDate] [datetime2](7) NOT NULL,
	[lmsCourseMeetingId] [int] NOT NULL,
 CONSTRAINT [PK_LmsCalendarEvent] PRIMARY KEY CLUSTERED 
(
	[lmsCalendarEventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LmsCalendarEvent]  WITH CHECK ADD  CONSTRAINT [FK_LmsCalendarEvent_LmsCourseMeeting] FOREIGN KEY([lmsCourseMeetingId])
REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
GO

ALTER TABLE [dbo].[LmsCalendarEvent] CHECK CONSTRAINT [FK_LmsCalendarEvent_LmsCourseMeeting]
GO