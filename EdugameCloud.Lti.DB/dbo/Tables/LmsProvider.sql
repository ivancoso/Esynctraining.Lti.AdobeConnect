CREATE TABLE [dbo].[LmsProvider] (
    [lmsProviderId]    INT            NOT NULL,
    [lmsProvider]      NVARCHAR (50)  NULL,
    [shortName]        NVARCHAR (50)  NULL,
    [configurationUrl] NVARCHAR (100) NULL,
    CONSTRAINT [PK_LmsProvider_1] PRIMARY KEY CLUSTERED ([lmsProviderId] ASC)
);





