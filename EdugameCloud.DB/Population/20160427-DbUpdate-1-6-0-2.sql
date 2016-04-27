ALTER TABLE [dbo].[LmsCourseMeeting]
ADD [EnableDynamicProvisioning] [bit] NOT NULL
CONSTRAINT DF_LmsCourseMeeting_EnableDynamicProvisioning DEFAULT 0