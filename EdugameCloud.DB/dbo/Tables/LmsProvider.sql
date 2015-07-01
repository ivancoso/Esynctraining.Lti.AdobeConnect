CREATE TABLE [dbo].[LmsProvider] (
    [lmsProviderId]    INT            NOT NULL,
    [lmsProvider]      NVARCHAR (50)  NOT NULL,
    [shortName]        NVARCHAR (50)  NOT NULL,
    [configurationUrl] NVARCHAR (100) NULL,
    [userGuideFileUrl] NVARCHAR (100) NULL,
    CONSTRAINT [PK_LmsProvider] PRIMARY KEY CLUSTERED ([lmsProviderId] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsProvider_lmsProvider] ON [dbo].[LmsProvider] ([lmsProvider])
GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsProvider_shortName] ON [dbo].[LmsProvider] ([shortName])
GO
