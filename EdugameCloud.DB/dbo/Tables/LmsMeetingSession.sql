CREATE TABLE [dbo].[LmsMeetingSession] (
    [lmsMeetingSessionId] INT             IDENTITY (1, 1) NOT NULL,
    [eventId]             NVARCHAR (50)   NULL,
    [name]                NVARCHAR (200)  NOT NULL,
    [startDate]           DATETIME2 (7)   NOT NULL,
    [endDate]             DATETIME2 (7)   NOT NULL,
    [lmsCourseMeetingId]  INT             NOT NULL,
    [summary]             NVARCHAR (2000) NULL,
    CONSTRAINT [PK_LmsMeetingSession] PRIMARY KEY CLUSTERED ([lmsMeetingSessionId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_LmsMeetingSession_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId])
);

