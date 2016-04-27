ALTER TABLE [dbo].[LmsCourseMeeting]
ADD [enableDynamicProvisioning] [bit] NOT NULL
CONSTRAINT DF_LmsCourseMeeting_EnableDynamicProvisioning DEFAULT 0