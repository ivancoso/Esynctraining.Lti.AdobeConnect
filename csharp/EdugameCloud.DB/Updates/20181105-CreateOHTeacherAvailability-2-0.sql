/****** Object:  Table [dbo].[OfficeHoursTeacherAvailability]    Script Date: 11/05/2018 1:57:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OfficeHoursTeacherAvailability](
	[officeHoursTeacherAvailabilityId] [int] IDENTITY(1,1) NOT NULL,
	[duration] [int] NOT NULL,
	[intervals] [nvarchar](1000) NOT NULL,
	[daysOfWeek] [nvarchar](20) NOT NULL,
	[periodStart] [datetime2](7) NOT NULL,
	[periodEnd] [datetime2](7) NOT NULL,
	[lmsUserId] [int] NOT NULL,
	[officeHoursId] [int] NOT NULL,
 CONSTRAINT [PK_OfficeHoursTeacherAvailability] PRIMARY KEY CLUSTERED 
(
	[officeHoursTeacherAvailabilityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OfficeHoursTeacherAvailability]  WITH CHECK ADD  CONSTRAINT [FK_OfficeHoursTeacherAvailability_LmsUser_lmsUserId] FOREIGN KEY([lmsUserId])
REFERENCES [dbo].[LmsUser] ([lmsUserId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OfficeHoursTeacherAvailability] CHECK CONSTRAINT [FK_OfficeHoursTeacherAvailability_LmsUser_lmsUserId]
GO

ALTER TABLE [dbo].[OfficeHoursTeacherAvailability]  WITH CHECK ADD  CONSTRAINT [FK_OfficeHoursTeacherAvailability_OfficeHours_officeHoursId] FOREIGN KEY([officeHoursId])
REFERENCES [dbo].[OfficeHours] ([officeHoursId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OfficeHoursTeacherAvailability] CHECK CONSTRAINT [FK_OfficeHoursTeacherAvailability_OfficeHours_officeHoursId]
GO

-------------------------------------------------------------------------

CREATE TABLE [dbo].[OfficeHoursSlot](
	[officeHoursSlotId] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NOT NULL,
	[start] [datetime2](7) NOT NULL,
	[end] [datetime2](7) NOT NULL,
	[subject] [nvarchar](200) NULL,
	[questions] [nvarchar](2000) NULL,
	[lmsUserId] [int] NULL,
	[availabilityId] [int] NOT NULL,
 CONSTRAINT [PK_OfficeHoursSlot] PRIMARY KEY CLUSTERED 
(
	[officeHoursSlotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OfficeHoursSlot]  WITH CHECK ADD  CONSTRAINT [FK_OfficeHoursSlot_LmsUser_lmsUserId] FOREIGN KEY([lmsUserId])
REFERENCES [dbo].[LmsUser] ([lmsUserId])
GO

ALTER TABLE [dbo].[OfficeHoursSlot] CHECK CONSTRAINT [FK_OfficeHoursSlot_LmsUser_lmsUserId]
GO

ALTER TABLE [dbo].[OfficeHoursSlot]  WITH CHECK ADD  CONSTRAINT [FK_OfficeHoursSlot_OfficeHoursTeacherAvailability_availabilityId] FOREIGN KEY([availabilityId])
REFERENCES [dbo].[OfficeHoursTeacherAvailability] ([officeHoursTeacherAvailabilityId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OfficeHoursSlot] CHECK CONSTRAINT [FK_OfficeHoursSlot_OfficeHoursTeacherAvailability_availabilityId]
GO
