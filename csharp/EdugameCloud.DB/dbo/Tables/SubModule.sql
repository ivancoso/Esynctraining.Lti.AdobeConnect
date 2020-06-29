CREATE TABLE [dbo].[SubModule] (
    [subModuleId]   INT           IDENTITY (1, 1) NOT NULL,
    [moduleID]      INT           NOT NULL,
    [subModuleName] VARCHAR (50)  NOT NULL,
    [dateCreated]   SMALLDATETIME CONSTRAINT [DF__SubModule__DateC__5AEE82B9] DEFAULT (getdate()) NOT NULL,
    [isActive]      BIT           CONSTRAINT [DF__SubModule__IsAct__5CD6CB2B] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_SubModule] PRIMARY KEY CLUSTERED ([subModuleId] ASC),
    CONSTRAINT [FK_SubModule_Module] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[Module] ([moduleId])
);

