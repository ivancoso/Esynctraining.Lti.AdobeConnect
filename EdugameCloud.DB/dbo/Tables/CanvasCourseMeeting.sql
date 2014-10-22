CREATE TABLE [dbo].[CanvasCourseMeeting] (
    [canvasCourseMeetingId]      INT           IDENTITY (1, 1) NOT NULL,
    [canvasConnectCredentialsId] INT           NOT NULL,
    [courseId]                   INT           NOT NULL,
    [scoId]                      NVARCHAR (50) NOT NULL,
    [companyLmsId]               INT           NULL,
    CONSTRAINT [PK_CanvasCourseMeeting] PRIMARY KEY CLUSTERED ([canvasCourseMeetingId] ASC),
    CONSTRAINT [FK_CanvasCourseMeeting_CanvasConnectCredentials] FOREIGN KEY ([canvasConnectCredentialsId]) REFERENCES [dbo].[CanvasConnectCredentials] ([canvasConnectCredentialsId])
);

