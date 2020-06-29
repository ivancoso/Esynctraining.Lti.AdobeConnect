CREATE TABLE [dbo].[SubModuleCategory] (
    [subModuleCategoryId] INT            IDENTITY (1, 1) NOT NULL,
    [userId]              INT            NOT NULL,
    [subModuleId]         INT            NOT NULL,
    [categoryName]        NVARCHAR (255) NULL,
    [modifiedBy]          INT            NULL,
    [dateModified]        SMALLDATETIME  CONSTRAINT [DF__UserSubMo__DateM__3A81B327] DEFAULT (getdate()) NOT NULL,
    [isActive]            BIT            CONSTRAINT [DF__UserSubMo__IsAct__3B75D760] DEFAULT ((0)) NULL,
    [lmsCourseId]         NVARCHAR (50)  NULL,
    [lmsProviderId]       INT            NULL,
    [companyLmsId]        INT            NULL,
    CONSTRAINT [PK_SubModuleCategory] PRIMARY KEY CLUSTERED ([subModuleCategoryId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_SubModuleCategory_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE SET NULL,
    CONSTRAINT [FK_SubModuleCategory_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]),
    CONSTRAINT [FK_SubModuleCategory_User] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]),
    CONSTRAINT [FK_UserSubModuleCategory_SubModule] FOREIGN KEY ([subModuleId]) REFERENCES [dbo].[SubModule] ([subModuleId]),
    CONSTRAINT [FK_UserSubModuleCategory_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId])
);













