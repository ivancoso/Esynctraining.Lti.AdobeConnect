CREATE TABLE [dbo].[LmsCourseSection] (
    [lmsCourseSectionId] INT            IDENTITY (1, 1) NOT NULL,
    [lmsCourseMeetingId] INT            NOT NULL,
    [lmsId]              NVARCHAR (50)  NOT NULL,
    [name]               NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_LmsCourseSection] PRIMARY KEY CLUSTERED ([lmsCourseSectionId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_LmsCourseSection_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
);

