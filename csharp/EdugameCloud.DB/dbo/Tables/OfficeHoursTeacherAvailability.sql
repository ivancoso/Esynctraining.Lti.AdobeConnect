CREATE TABLE [dbo].[OfficeHoursTeacherAvailability] (
    [officeHoursTeacherAvailabilityId] INT             IDENTITY (1, 1) NOT NULL,
    [duration]                         INT             NOT NULL,
    [intervals]                        NVARCHAR (1000) NOT NULL,
    [daysOfWeek]                       NVARCHAR (20)   NOT NULL,
    [periodStart]                      DATETIME2 (7)   NOT NULL,
    [periodEnd]                        DATETIME2 (7)   NOT NULL,
    [lmsUserId]                        INT             NOT NULL,
    [officeHoursId]                    INT             NOT NULL,
    CONSTRAINT [PK_OfficeHoursTeacherAvailability] PRIMARY KEY CLUSTERED ([officeHoursTeacherAvailabilityId] ASC),
    CONSTRAINT [FK_OfficeHoursTeacherAvailability_LmsUser_lmsUserId] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfficeHoursTeacherAvailability_OfficeHours_officeHoursId] FOREIGN KEY ([officeHoursId]) REFERENCES [dbo].[OfficeHours] ([officeHoursId]) ON DELETE CASCADE
);

