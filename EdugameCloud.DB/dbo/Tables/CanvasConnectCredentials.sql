CREATE TABLE [dbo].[CanvasConnectCredentials] (
    [canvasConnectCredentialsId] INT            IDENTITY (1, 1) NOT NULL,
    [canvasDomain]               NVARCHAR (50)  NOT NULL,
    [acDomain]                   NVARCHAR (50)  NOT NULL,
    [acUsername]                 NVARCHAR (50)  NOT NULL,
    [acPassword]                 NVARCHAR (50)  NOT NULL,
    [acScoId]                    NVARCHAR (10)  NULL,
    [canvasToken]                NVARCHAR (100) NULL,
    [acTemplateScoId]            NVARCHAR (50)  NULL,
    CONSTRAINT [PK_CanvasConnectCredentials] PRIMARY KEY CLUSTERED ([canvasConnectCredentialsId] ASC)
);

