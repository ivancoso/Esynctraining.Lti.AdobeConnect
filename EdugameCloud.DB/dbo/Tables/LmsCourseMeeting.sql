CREATE TABLE [dbo].[LmsCourseMeeting] (
    [lmsCourseMeetingId] INT            IDENTITY (1, 1) NOT NULL,
    [courseId]           INT            NOT NULL,
    [scoId]              NVARCHAR (50)  NULL,
    [companyLmsId]       INT            NULL,
    [lmsMeetingTypeId]   INT            NULL,
    [cachedUsers]        NVARCHAR (MAX) NULL,
    [addedToCache]       DATETIME       NULL,
    [officeHoursId]      INT            NULL,
    [ownerId]            INT            NULL,
    CONSTRAINT [PK_CanvasCourseMeeting] PRIMARY KEY CLUSTERED ([lmsCourseMeetingId] ASC),
    CONSTRAINT [FK_LmsCourseMeeting_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
);





