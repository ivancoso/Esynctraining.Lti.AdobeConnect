CREATE TABLE [dbo].[CanvasACMeeting] (
    [canvasACMeetingId] INT            IDENTITY (1, 1) NOT NULL,
    [contextId]         NVARCHAR (MAX) NOT NULL,
    [scoId]             NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_CanvasACMeeting] PRIMARY KEY CLUSTERED ([canvasACMeetingId] ASC)
);

