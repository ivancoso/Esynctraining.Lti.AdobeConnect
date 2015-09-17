CREATE TABLE [dbo].[LmsUserMeetingRole] (
    [lmsUserMeetingRoleId] INT           IDENTITY (1, 1) NOT NULL,
    [lmsUserId]            INT           NOT NULL,
    [lmsCourseMeetingId]   INT           NOT NULL,
    [lmsRole]              NVARCHAR (50) NULL,
    CONSTRAINT [PK_LmsUserMeetingRole_1] PRIMARY KEY CLUSTERED ([lmsUserMeetingRoleId] ASC),
    CONSTRAINT [FK_LmsUserMeetingRole_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_LmsUserMeetingRole_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]) ON DELETE CASCADE ON UPDATE CASCADE
);



