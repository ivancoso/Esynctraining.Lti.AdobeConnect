CREATE TABLE [dbo].[SubModuleItem] (
    [subModuleItemId]     INT           IDENTITY (1, 1) NOT NULL,
    [subModuleCategoryId] INT           CONSTRAINT [DF_SubModuleItem_subModuleCategoryId] DEFAULT ((0)) NOT NULL,
    [createdBy]           INT           NULL,
    [isShared]            BIT           CONSTRAINT [DF__SubModule__IsSha__3E52440B] DEFAULT ((0)) NULL,
    [modifiedBy]          INT           NULL,
    [dateCreated]         SMALLDATETIME CONSTRAINT [DF__SubModule__DateC__3F466844] DEFAULT (getdate()) NOT NULL,
    [dateModified]        SMALLDATETIME CONSTRAINT [DF__SubModule__DateM__403A8C7D] DEFAULT (getdate()) NOT NULL,
    [isActive]            BIT           CONSTRAINT [DF__SubModule__IsAct__412EB0B6] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_SubModuleItem] PRIMARY KEY CLUSTERED ([subModuleItemId] ASC),
    CONSTRAINT [FK_SubModuleItem_SubModuleCategory] FOREIGN KEY ([subModuleCategoryId]) REFERENCES [dbo].[SubModuleCategory] ([subModuleCategoryId])
);







