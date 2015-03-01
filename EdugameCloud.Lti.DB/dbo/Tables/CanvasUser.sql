CREATE TABLE [dbo].[CanvasUser] (
    [canvasUserId]  INT           IDENTITY (1, 1) NOT NULL,
    [canvasId]      NVARCHAR (15) NOT NULL,
    [acUserName]    NVARCHAR (20) NOT NULL,
    [acPassword]    NVARCHAR (20) NOT NULL,
    [acFolderScoId] NVARCHAR (20) NULL,
    CONSTRAINT [PK_CanvasUser] PRIMARY KEY CLUSTERED ([canvasUserId] ASC)
);

