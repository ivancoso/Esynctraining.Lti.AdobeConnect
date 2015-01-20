CREATE TABLE [dbo].[LmsCourseMeeting] (
    [lmsCourseMeetingId] INT           IDENTITY (1, 1) NOT NULL,
    [courseId]           INT           NOT NULL,
    [scoId]              NVARCHAR (50) NULL,
    [companyLmsId]       INT           NULL,
    [cachedUsers]        NTEXT         NULL,
    [addedToCache]       DATETIME      NULL,
    [officeHoursId]      INT           NULL,
    [ownerId]            INT           NULL,
    [lmsMeetingTypeId]   INT           NULL,
    CONSTRAINT [PK_LmsCourseMeeting] PRIMARY KEY CLUSTERED ([lmsCourseMeetingId] ASC)
);





