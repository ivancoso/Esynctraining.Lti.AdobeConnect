CREATE TABLE [dbo].[AppletItem] (
    [appletItemId]    INT           IDENTITY (1, 1) NOT NULL,
    [subModuleItemId] INT           NULL,
    [appletName]      VARCHAR (50)  NOT NULL,
    [documentXML]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK_AppletItem] PRIMARY KEY CLUSTERED ([appletItemId] ASC),
    CONSTRAINT [FK_AppletItem_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId])
);

