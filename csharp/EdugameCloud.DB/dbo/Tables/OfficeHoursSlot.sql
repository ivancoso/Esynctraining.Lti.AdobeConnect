CREATE TABLE [dbo].[OfficeHoursSlot] (
    [officeHoursSlotId] INT             IDENTITY (1, 1) NOT NULL,
    [status]            INT             NOT NULL,
    [start]             DATETIME2 (7)   NOT NULL,
    [end]               DATETIME2 (7)   NOT NULL,
    [subject]           NVARCHAR (200)  NULL,
    [questions]         NVARCHAR (2000) NULL,
    [lmsUserId]         INT             NULL,
    [availabilityId]    INT             NOT NULL,
    CONSTRAINT [PK_OfficeHoursSlot] PRIMARY KEY CLUSTERED ([officeHoursSlotId] ASC),
    CONSTRAINT [FK_OfficeHoursSlot_LmsUser_lmsUserId] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]),
    CONSTRAINT [FK_OfficeHoursSlot_OfficeHoursTeacherAvailability_availabilityId] FOREIGN KEY ([availabilityId]) REFERENCES [dbo].[OfficeHoursTeacherAvailability] ([officeHoursTeacherAvailabilityId]) ON DELETE CASCADE
);

