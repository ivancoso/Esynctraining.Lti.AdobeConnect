CREATE TABLE [dbo].[Schedule] (
    [scheduleId]         INT      IDENTITY (1, 1) NOT NULL,
    [interval]           INT      NOT NULL,
    [nextRun]            DATETIME NOT NULL,
    [scheduleDescriptor] INT      NOT NULL,
    [scheduleType]       INT      NOT NULL,
    [isEnabled]          BIT      NOT NULL,
    CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED ([scheduleId] ASC)
);

