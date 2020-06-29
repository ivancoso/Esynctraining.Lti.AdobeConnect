CREATE TABLE [dbo].[LmsCourseMeeting] (
    [lmsCourseMeetingId]        INT             IDENTITY (1, 1) NOT NULL,
    [courseId]                  NVARCHAR (50)   NOT NULL,
    [scoId]                     NVARCHAR (50)   NULL,
    [companyLmsId]              INT             NOT NULL,
    [lmsMeetingTypeId]          INT             NOT NULL,
    [officeHoursId]             INT             NULL,
    [ownerId]                   INT             NULL,
    [meetingNameJson]           NVARCHAR (4000) NULL,
    [reused]                    BIT             NULL,
    [sourceCourseMeetingId]     INT             NULL,
    [audioProfileId]            NVARCHAR (50)   NULL,
    [enableDynamicProvisioning] BIT             CONSTRAINT [DF_LmsCourseMeeting_enableDynamicProvisioning] DEFAULT ((0)) NOT NULL,
    [audioProfileProvider]      NVARCHAR (50)   NULL,
    [lmsCalendarEventId]        NVARCHAR (50)   NULL,
    CONSTRAINT [PK_LmsCourseMeeting] PRIMARY KEY CLUSTERED ([lmsCourseMeetingId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_LmsCourseMeeting_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]),
    CONSTRAINT [FK_LmsCourseMeeting_LmsMeetingType] FOREIGN KEY ([lmsMeetingTypeId]) REFERENCES [dbo].[LmsMeetingType] ([lmsMeetingTypeId]),
    CONSTRAINT [FK_LmsCourseMeeting_LmsUser] FOREIGN KEY ([ownerId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]),
    CONSTRAINT [FK_LmsCourseMeeting_OfficeHours] FOREIGN KEY ([officeHoursId]) REFERENCES [dbo].[OfficeHours] ([officeHoursId])
);




GO
