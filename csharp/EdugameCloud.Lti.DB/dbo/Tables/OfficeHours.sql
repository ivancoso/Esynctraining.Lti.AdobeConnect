CREATE TABLE [dbo].[OfficeHours] (
    [officeHoursId] INT            IDENTITY (1, 1) NOT NULL,
    [hours]         NVARCHAR (100) NULL,
    [scoId]         NVARCHAR (50)  NOT NULL,
    [lmsUserId]     INT            NOT NULL,
    CONSTRAINT [PK_OfficeHours] PRIMARY KEY CLUSTERED ([officeHoursId] ASC)
);

