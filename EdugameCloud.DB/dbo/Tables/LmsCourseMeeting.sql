CREATE TABLE [dbo].[LmsCourseMeeting] (
    [lmsCourseMeetingId] INT           IDENTITY (1, 1) NOT NULL,
    [courseId]           INT           NOT NULL,
    [scoId]              NVARCHAR (50) NOT NULL,
    [companyLmsId]       INT           NULL,
    CONSTRAINT [PK_CanvasCourseMeeting] PRIMARY KEY CLUSTERED ([lmsCourseMeetingId] ASC)
);

